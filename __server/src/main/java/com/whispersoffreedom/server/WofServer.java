package com.whispersoffreedom.server;

import com.whispersoffreedom.server.command.WofCommand;
import com.whispersoffreedom.server.exception.*;
import com.whispersoffreedom.server.packet.WofPacket;
import lombok.Getter;
import lombok.NonNull;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import javax.annotation.PostConstruct;
import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.time.Instant;
import java.time.temporal.ChronoUnit;
import java.util.*;

@Service
public class WofServer {

    @Getter
    private static boolean initialized = false;

    private static Hashtable<String, Battle> battles = new Hashtable<>();

    private static Hashtable<String, Client> clients = new Hashtable<>();

    private static Logger logger = LoggerFactory.getLogger(WofServer.class);

    private static TcpServer tcpServer;

    private static UdpServer udpServer;

    @Autowired
    private AdminService adminServiceAutowire;

    private static AdminService adminService;

    /**
     * Maximum number of connected clients allowed
     */
    private static final int MAX_CLIENTS = 20;

    /**
     * Time in seconds after which client will be dropped
     * if no TCP connection has been established
     */
    private static final int MAX_ZOMBIE_TIME = 30;

    @PostConstruct
    private void autowiredInit() {
        adminService = this.adminServiceAutowire;
    }

    public static void initializeServer() throws IOException {
        if (initialized)
            throw new ServerAlreadyInitializedException();
        new Thread(() -> tcpServer = new TcpServer()).start();
        udpServer = new UdpServer();
        new Thread(() -> udpServer.startListening()).start();
        new Thread(WofServer::dropZombies).start();
        initialized = true;
    }

    public static Battle getBattle(String battleId) {
        if (!battles.containsKey(battleId))
            throw new BattleNotFoundException();

        return battles.get(battleId);
    }

    public static Client getClient(String clientId) {
        if (!clients.containsKey(clientId))
            throw new ClientNotFoundException();

        return clients.get(clientId);
    }

    public static Client registerClient(String username) {
        Client c = new Client(username);
        clients.put(c.getId().toString(), c);

        return c;
    }

    public static Battle initializeBattle(String userToken) {
        Battle newBattle = new Battle();
        Client c = getClient(userToken);
        if (c.isInBattle())
            throw new AlreadyInBattleException();
        c.enterBattle(newBattle);
        newBattle.getClients().put(c.getId().toString(), c);
        battles.put(newBattle.getBattleId(), newBattle);
        logger.info(String.format("Client %s is starting a battle.", c.getUsername()));

        return battles.get(newBattle.getBattleId());
    }

    public static void clientLeavesBattle(Client client) {
        if (!client.isInBattle())
            throw new NotInBattleException();
        client.getCurrentBattle().broadcast(
                String.format("Client %s is disconnecting from battle.", client.getUsername()));
        String battleId = client.getCurrentBattle().getBattleId();
        client.getCurrentBattle().clientLeaves(client);
        client.leaveBattle();
        Battle b = getBattle(battleId);
        if (b.getClients().size() == 0)
            battles.remove(battleId);
    }

    public static List<Battle> getOpenLobbies() {
        Iterator i = battles.entrySet().iterator();
        ArrayList<Battle> result = new ArrayList<>();

        while (i.hasNext()) {
            Map.Entry entry = (Map.Entry) i.next();
            Battle b = (Battle) entry.getValue();
            if (b.getStatus().equals(BattleStatus.LOBBY))
                result.add(b);
        }

        return result;
    }

    public static void renameBattle(@NonNull Client client, @NonNull String newName) {
        if (client.getCurrentBattle() == null || !client.isInBattle())
            throw new BattleNotFoundException();
        client.getCurrentBattle().rename(client, newName);
    }

    public static void clientEntersBattle(Client c, Battle b) {
        if (c.isInBattle())
            throw new AlreadyInBattleException();
        if (!b.getStatus().equals(BattleStatus.LOBBY))
            throw new CannotJoinBattleException();
        c.enterBattle(b);
        b.clientEnters(c);
    }

    public static void clientDataReceived(TcpConnection connection, WofPacket packet) {
        try {
            WofCommand command = WofCommand.build(connection, packet);
            command.execute();
        } catch (InvalidCommandReceived ex) {
            logger.error(String.format("Unhandled command %s received.", packet.getCommand()));
        }
    }

    /**
     * Connects to Admin API to verify credentials
     *
     * @param username email
     * @param password password
     * @return true if credentials are correct
     */
    public static boolean enterServer(String username, String password) {
        logger.info(String.format("User %s is entering the server...", username));
        return adminService.verifyCredentials(username, password);
    }

    public static boolean isFull() {
        return getClientCount() >= MAX_CLIENTS;
    }

    public static int getBattleCount() {
        return battles.size();
    }

    public static int getClientCount() {
        return clients.size();
    }

    public static List<Client> getAllClients() {
        ArrayList<Client> result = new ArrayList<>();
        for (Map.Entry<String, Client> entry : clients.entrySet()) {
            result.add(entry.getValue());
        }

        return result;
    }

    /**
     * Destroys and removes client by connection ID
     *
     * @param connectionId UUID of the connection
     */
    public static void dropClientByConnectionId(UUID connectionId) {
        String clientId = getClientIdByConnectionId(connectionId);
        dropClientById(clientId);
    }

    /**
     * Destroy client object and remove it from the list of connected client
     *
     * @param clientId string UUID of the client
     */
    public static void dropClientById(String clientId) {
        Client client = clients.get(clientId);
        logger.info("Dropping " + client.getUsername());
        if (client.isInBattle())
            clientLeavesBattle(client);
        clients.get(clientId).destroy();
        clients.remove(clientId);
    }

    /**
     * Inefficient method of finding client's ID
     * based on connection ID. Used only for dropping
     * clients in case TCP connection has been lost.
     *
     * @param connectionId unique identifier of the connection
     * @return string UUID of the client
     */
    private static String getClientIdByConnectionId(UUID connectionId) throws ClientNotFoundException {
        for (Map.Entry<String, Client> entry : clients.entrySet())
            if (entry.getValue().getConnection() != null)
                if (entry.getValue().getConnection().getConnectionId() == connectionId)
                    return entry.getKey();
        throw new ClientNotFoundException();
    }

    /**
     * Iterates through connected clients
     *
     * @param username user identifier
     * @return found client or null
     */
    public static Client getClientByUsername(String username) {
        for (Map.Entry<String, Client> entry : clients.entrySet()) {
            if (entry.getValue().getUsername().equalsIgnoreCase(username))
                return entry.getValue();
        }

        return null;
    }

    /**
     * Called by UdpServer whenever client sends
     * identification packet synchronizing client
     * with UDP server allowing broadcast
     *
     * @param wofPacket received packet
     * @param socket    UDP socket instance
     */
    public static void clientIdentified(DatagramSocket socket, WofPacket wofPacket, DatagramPacket packet) {
        Client c = clients.get(wofPacket.getClientId());
        c.acceptUdpConnection(socket, packet);
        logger.info(String.format("Client %s connected via UDP from %s", c.getUsername(), packet.getAddress().toString()));
    }

    /**
     * Once client established a connection, it needs to identify it
     * by providing its session token (client ID)
     *
     * @param connection valid TCP/IP connection established by the client
     * @param packet     Usually an identification packet but it can be any packet
     */
    public static void clientIdentifiedTcp(TcpConnection connection, WofPacket packet) {
        Client c = clients.get(packet.getClientId());
        c.acceptTcpConnection(connection);
        logger.info(String.format("Client %s connected via TCP", c.getUsername()));
    }

    /**
     * Each client is obligated to establish TCP connection
     * with the server within MAX_ZOMBIE_TIME otherwise
     * REST session will be terminated and client will have
     * to start a new session. Unfortunately if there is no TCP
     * connection there is no way of informing the client that
     * the session has been terminated.
     */
    private static void dropZombies() {
        while (true) {
            try {
                Thread.sleep(10000);
            } catch (InterruptedException e) {
                logger.error(e.getMessage());
            }
            clients.entrySet().removeIf(c -> isZombie((c.getValue())));
        }
    }

    /**
     * Returns true if the client is a zombie
     * and should be dropped.
     *
     * @param c Given client
     * @return true if c is a zombie
     */
    private static boolean isZombie(Client c) {
        if (c.getConnection() != null)
            return false;

        return c.getConnected()
                .plus(MAX_ZOMBIE_TIME, ChronoUnit.SECONDS)
                .isBefore(Instant.now());
    }

    /**
     * Called by the clients that want to disconnect.
     * Removes client session and cleans up.
     * If the client does not call explicitly, it is removed
     * automatically if client stops heart-beating
     *
     * @param sessionToken session token generated by the client
     */
    public static void leaveServer(String sessionToken) {
        logger.info(String.format("Client %s is disconnecting...", sessionToken));
        getClient(sessionToken).destroy();
        dropClientById(sessionToken);
    }
}
