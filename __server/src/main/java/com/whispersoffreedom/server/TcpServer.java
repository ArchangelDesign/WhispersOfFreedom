package com.whispersoffreedom.server;

import lombok.Getter;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;

import java.net.*;
import java.io.*;
import java.util.HashMap;
import java.util.UUID;

public class TcpServer {

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
                logger.info("received: " + inputLine);
                out.println(inputLine);
            }
        } catch (Exception e) {
            logger.error(e.getMessage());
            return;
        }
    }

    /**
     * Checks active connections and drops idle/lost ones
     */
    private void watchdog() {
        // @TODO
        try {
            Thread.sleep(30000);
        } catch (InterruptedException e) {
            e.printStackTrace();
        }
        logger.info("checking lost connections...");
    }
}
