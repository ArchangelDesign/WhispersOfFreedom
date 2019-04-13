package com.whispersoffreedom.server;

import lombok.Getter;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;

import java.net.*;
import java.io.*;
import java.time.Instant;
import java.time.temporal.ChronoUnit;
import java.util.HashMap;
import java.util.UUID;

public class TcpServer {

    private final int CONNECTION_TIMEOUT_MIN = 10;

    @Getter
    private Integer port = 8081;

    private ServerSocket serverSocket;

    private Logger logger = LoggerFactory.getLogger(TcpServer.class);

    private HashMap<UUID, TcpConnection> connections = new HashMap<>();

    public TcpServer() throws IOException {
        logger.info("Starting TCP server...");
        serverSocket = new ServerSocket(port);
        logger.info("TCP server started.");
        new Thread(this::watchdog).start();
        while (true)
            acceptConnection(serverSocket.accept());
    }

    private void acceptConnection(Socket socket) {
        logger.info(String.format("Accepting connection from %s...", socket.getRemoteSocketAddress()));
        new Thread(() -> onConnection(socket)).start();
    }

    private void onConnection(Socket socket) {
        UUID id = UUID.randomUUID();
        TcpConnection connection = new TcpConnection(id, socket);
        connections.put(id, connection);
        PrintWriter out;
        BufferedReader in;
        try {
            out = new PrintWriter(socket.getOutputStream(), true);
            in = new BufferedReader(
                    new InputStreamReader(socket.getInputStream()));

            String inputLine;

            while ((inputLine = in.readLine()) != null) {
                sendAck(out);
                connection.rawPacketReceived(inputLine);
            }
        } catch (SocketException e) {
            logger.error("Socket Error: " + e.getMessage());
            logger.error("Client ID: " + id.toString());
        } catch (Exception e) {
            logger.error("Error reading data from " + socket.getRemoteSocketAddress());
            e.printStackTrace();
        }

        logger.info("Client dropped " + socket.getRemoteSocketAddress());
        WofServer.clientDropped(connection.getConnectionId());
        connections.remove(id);
    }

    private void sendAck(PrintWriter out) {
        // @TODO
        out.println("{\"command\":\"ack\"}");
    }

    /**
     * Checks active connections and drops idle/lost ones
     */
    private void watchdog() {
        // @TODO
        while (true) {
            try {
                Thread.sleep(30000);
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
            connections.forEach((uuid, tcpConnection) -> {
                if (tcpConnection.getLastTransmission().
                        isBefore(Instant.now().minus(CONNECTION_TIMEOUT_MIN, ChronoUnit.MINUTES))) {
                    logger.info(String.format("Dropping %s due to timeout.", uuid));
                    dropConnection(uuid);
                    return;
                }
                tcpConnection.sendPacket(new WofPacket().setCommand("ping"));
            });
        }
    }

    private void dropConnection(UUID id) {
        new Thread(() -> {
            try {
                Thread.sleep(100);
                connections.get(id).getSocket().close();
                connections.remove(id);
            } catch (InterruptedException | IOException e) {
            }
        }).start();
    }
}
