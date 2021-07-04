package com.whispersoffreedom.server;

import com.whispersoffreedom.server.packet.StateUpdatePacket;
import com.whispersoffreedom.server.packet.WofPacket;
import com.whispersoffreedom.server.packet.WofPacketBroadcast;
import lombok.Getter;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
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

    private DatagramSocket udpSocket;

    private boolean alive = true;

    @Getter
    private Instant connected = Instant.now();

    Logger logger;
    private DatagramPacket udpPacket;

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
        sendTcpPacket(new WofPacketBroadcast(id.toString(), message));
    }

    public void sendTcpPacket(WofPacket packet) {
        connection.sendPacket(packet);
    }

    public void sendUdpPacket(WofPacket packet) throws IOException {
        if (udpSocket == null)
            return;
        String data = packet.toJson();
        udpPacket.setData(data.getBytes());
        udpSocket.send(udpPacket);
    }

    public void acceptTcpConnection(TcpConnection conn) {
        logger.info(this.username + " is accepting TCP connection from " + conn.getRemoteAddress());
        connection = conn;
        conn.setClient(this);
    }

    public void acceptUdpConnection(DatagramSocket socket, DatagramPacket packet) {
        logger.info("Accepting UDP connections on " + socket.toString());
        udpSocket = socket;
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

            if (udpSocket == null)
                continue;

            try {
                sendUdpPacket(new StateUpdatePacket(this));
            } catch (IOException e) {
                logger.error("UDP Heartbeat error: " + e.getMessage());
            }
        }
    }
}
