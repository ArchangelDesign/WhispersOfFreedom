package com.whispersoffreedom.server;

import com.whispersoffreedom.server.exception.ClientNotFoundException;
import com.whispersoffreedom.server.packet.AckPacket;
import com.whispersoffreedom.server.packet.PingPacket;
import lombok.Getter;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.net.*;
import java.io.*;
import java.time.Instant;
import java.time.temporal.ChronoUnit;
import java.util.HashMap;
import java.util.Hashtable;
import java.util.UUID;

public class TcpServer {

    private final int CONNECTION_TIMEOUT_MIN = 10;

    @Getter
    private Integer port = 8081;

    private ServerSocket serverSocket;

    private boolean running = true;

    private Logger logger = LoggerFactory.getLogger(TcpServer.class);

    private Hashtable<UUID, TcpConnection> connections = new Hashtable<>();

    public TcpServer() {
        logger.info("Starting TCP server...");
        try {
            serverSocket = new ServerSocket(port);
        } catch (IOException e) {
            e.printStackTrace();
        }
        logger.info("TCP server started.");
        new Thread(this::watchdog).start();
        while (running) {
            try {
                acceptConnection(serverSocket.accept());
            } catch (IOException e) {
                logger.error(e.getMessage());
            }
        }
        logger.error("TCP server terminated");
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

        logger.info(
                String.format(
                        "Dropping client %s due to stream termination or network error.",
                        socket.getRemoteSocketAddress())
        );
        try {
            WofServer.dropClientByConnectionId(connection.getConnectionId());
        } catch (ClientNotFoundException e) {
            logger.info("Client has not been assigned entity, will be dropped due to timeout.");
        } finally {
            connections.remove(id);
        }
    }

    private void sendAck(PrintWriter out) {
        AckPacket packet = new AckPacket();
        out.println(packet.toJson());
        out.flush(); // not sure if it's needed here
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
            }
            connections.forEach((uuid, tcpConnection) -> {
                if (tcpConnection.getLastTransmission().
                        isBefore(Instant.now().minus(CONNECTION_TIMEOUT_MIN, ChronoUnit.MINUTES))) {
                    logger.info(String.format("Dropping %s due to timeout.", uuid));
                    dropConnection(uuid);
                    return;
                }
                if (tcpConnection.getClient() == null) {
                    // Issue: TCP connection keeps sending packets after the client is no longer
                    // on the list. Drop the connection as soon as getClient() returns null
                    logger.info(String.format("Dropping connection %s since client has been dropped.", uuid));
                    dropConnection(uuid);
                }
                tcpConnection.sendPacket(new PingPacket());
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
