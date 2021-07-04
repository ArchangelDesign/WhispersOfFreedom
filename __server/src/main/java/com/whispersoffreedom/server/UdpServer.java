package com.whispersoffreedom.server;

import com.google.gson.Gson;
import com.google.gson.JsonSyntaxException;
import com.whispersoffreedom.server.packet.PingPacket;
import com.whispersoffreedom.server.packet.WofPacket;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.util.HashMap;
import java.util.Iterator;
import java.util.Map;

public class UdpServer {

    private static final int port = 8082;

    private static final String address = "localhost";

    private boolean running = true;

    private Logger logger = LoggerFactory.getLogger(UdpServer.class);

    private final HashMap<String, DatagramPacket> clients = new HashMap<>();

    private DatagramSocket socket;

    public UdpServer() {
        try {
            socket = new DatagramSocket(port);
            new Thread(this::watchdog).start();
        } catch (IOException e) {
            logger.error(e.getMessage());
        }
    }

    public void startListening() {
        byte[] buffer = new byte[255];
        logger.info("UDP server started.");
        while (running) {
            DatagramPacket packet = new DatagramPacket(buffer, buffer.length);
            try {
                socket.receive(packet);
            } catch (IOException e) {
                logger.error(e.getMessage());
                continue;
            }
            onDataReceived(packet);
        }
        logger.error("UDP server stopped listening.");
    }

    private void onDataReceived(DatagramPacket packet) {
        String data = new String(packet.getData(), 0, packet.getLength());
        logger.info(String.format("[%s] UDP packet received: %s", packet.getSocketAddress().toString(), data));
        WofPacket wofPacket = null;
        try {
            wofPacket = new Gson().fromJson(data, WofPacket.class);
        } catch (JsonSyntaxException ex) {
            logger.error("Invalid JSON received via UDP. ");
            return;
        }
        if (!clients.containsKey(packet.getSocketAddress().toString())) {
            logger.info("New UDP client " + packet.getSocketAddress().toString());
            clients.put(packet.getSocketAddress().toString(), packet);
        }

        if (wofPacket.isIdentification()) {
            // this is the only packet we care about for now
            logger.info("Identifying UDP client...");
            WofServer.clientIdentified(socket, wofPacket, packet);
        }

    }

    private void watchdog() {

    }

    public void send(DatagramPacket dp) throws IOException {
        socket.send(dp);
    }
}
