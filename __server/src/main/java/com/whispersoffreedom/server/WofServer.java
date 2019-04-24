package com.whispersoffreedom.server;

import com.whispersoffreedom.server.command.WofCommand;
import com.whispersoffreedom.server.exception.*;
import lombok.Getter;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.IOException;
import java.util.*;

public class WofServer {

    @Getter
    private static boolean initialized = false;

    private static HashMap<String, Battle> battles = new HashMap<>();

    private static HashMap<String, Client> clients = new HashMap<>();

    private static Logger logger = LoggerFactory.getLogger(WofServer.class);

    private static TcpServer tcpServer;

    private static final int MAX_CLIENTS = 20;

    public static void initializeServer() throws IOException {
        if (initialized)
            throw new ServerAlreadyInitializedException();
        tcpServer = new TcpServer();
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

    public static void renameBattle(Client client, String newName) {
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
        WofCommand command = WofCommand.build(connection, packet);
        command.execute();
    }

    public static void enterServer(String username) {
        logger.info(String.format("User %s is entering the server...", username));
    }

    public static boolean isFull() {
        return getClientCount() >= MAX_CLIENTS;
    }

    public static int getClientCount() {
        return clients.size();
    }

    public static void clientDropped(UUID connectionId) {
        String clientId = getClientIdByConnectionId(connectionId);
        Client client = clients.get(clientId);
        logger.info("Dropping " + client.getUsername());
        if (client.isInBattle())
            clientLeavesBattle(client);
        clients.remove(clientId);
    }

    public static void clientIdentified(TcpConnection connection, WofPacket packet) {
        getClient(packet.getClientId()).acceptTcpConnection(connection);
    }

    private static String getClientIdByConnectionId(UUID connectionId) {
        for (Map.Entry<String, Client> entry : clients.entrySet())
            if (entry.getValue().getConnection().getConnectionId() == connectionId)
                return entry.getKey();
        throw new ClientNotFoundException();
    }
}
