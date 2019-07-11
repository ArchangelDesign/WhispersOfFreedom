package com.whispersoffreedom.server;

import com.whispersoffreedom.server.packet.PingPacket;
import com.whispersoffreedom.server.packet.StateUpdatePacket;
import com.whispersoffreedom.server.packet.WofPacket;
import com.whispersoffreedom.server.packet.WofPacketBroadcast;
import lombok.Getter;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.net.DatagramPacket;
import java.time.Instant;
import java.util.UUID;

public class Client {

    @Getter
    private UUID id = UUID.randomUUID();

    @Getter
    private String username;

    @Getter
    private boolean inBattle = false;

    @Getter
    private Battle currentBattle = null;

    @Getter
    private Battle previousBattle = null;

    private TcpConnection connection;

    private DatagramPacket udpPacket;

    private boolean alive = true;

    @Getter
    private Instant connected = Instant.now();

    Logger logger;

    public Client(String newUsername) {
        username = newUsername;
        logger = LoggerFactory.getLogger(String.format("Client [%s]", newUsername));
        new Thread(this::heartbeat).start();
    }

    public void enterBattle(Battle battle) {
        currentBattle = battle;
        inBattle = true;
    }

    public void leaveBattle() {
        previousBattle = currentBattle;
        currentBattle = null;
        inBattle = false;
    }

    public void broadcast(String message) {
        logger.info("Broadcasting message: " + message);
        if (connection == null) {
            logger.error("No TCP connection for " + username);
            return;
        }
        sendPacket(new WofPacketBroadcast(id.toString(), message));
    }

    public void sendPacket(WofPacket packet) {
        connection.sendPacket(packet);
    }

    public void sendUdpPacket(WofPacket packet) {
        if (udpPacket == null)
            return;
        String data = packet.toJson();
        DatagramPacket dp = new DatagramPacket(data.getBytes(), data.length(), udpPacket.getSocketAddress());
        WofServer.sendUdpPacket(dp);
    }

    public void acceptTcpConnection(TcpConnection conn) {
        logger.info(this.username + " is accepting TCP connection from " + conn.getRemoteAddress());
        connection = conn;
        conn.setClient(this);
    }

    public void acceptUdpConnection(DatagramPacket packet) {
        logger.info("Accepting UDP connections on " + packet.getSocketAddress().toString());
        udpPacket = packet;
    }

    public TcpConnection getConnection() {
        return connection;
    }

    public void destroy() {
        alive = false;
    }

    /**
     * Synchronization method. If in battle runs very often and
     * sends current state to the client. Each client has their
     * own thread to handle synchronization.
     */
    private void heartbeat() {
        while (alive) {
            long delay = inBattle ? 300 : 5000;
            try {
                Thread.sleep(delay);
            } catch (InterruptedException e) {
            }

//            if (udpPacket == null && inBattle) {
//                logger.error(String.format("Client %s is in battle and is not connected via UDP. Dropping...", username));
//                WofServer.dropClientById(id.toString());
//                return;
//            }

            if (udpPacket == null)
                continue;

            String buff = new StateUpdatePacket(this).toJson();
            udpPacket.setData(buff.getBytes());
            WofServer.sendUdpPacket(udpPacket);
        }
    }
}
