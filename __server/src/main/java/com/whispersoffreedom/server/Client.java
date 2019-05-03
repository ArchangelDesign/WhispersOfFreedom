package com.whispersoffreedom.server;

import lombok.Getter;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
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

    Logger logger;

    public Client(String newUsername) {
        username = newUsername;
        logger = LoggerFactory.getLogger(String.format("Client [%s]", newUsername));
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
}
