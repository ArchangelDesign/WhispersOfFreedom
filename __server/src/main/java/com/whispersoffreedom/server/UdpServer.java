package com.whispersoffreedom.server;

import com.google.gson.Gson;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.util.ArrayList;
import java.util.HashMap;

public class UdpServer {

    private static final int port = 8082;

    private static final String address = "localhost";

    private boolean running = true;

    private Logger logger = LoggerFactory.getLogger(UdpServer.class);

    private HashMap<String, DatagramPacket> clients = new HashMap<>();

    private DatagramSocket socket;

    public UdpServer() {
        try {
            socket = new DatagramSocket(port);
            new Thread(this::watchdog).start();
        } catch (IOException e) {
            logger.error(e.getMessage());
        }
        logger.error("UDP server stopped listening.");
    }

    public void startListening() {
        byte[] buffer = new byte[255];

        while (running) {
            logger.info("waiting for packet...");
            DatagramPacket packet = new DatagramPacket(buffer, buffer.length);

            try {
                socket.receive(packet);
            } catch (IOException e) {
                logger.error(e.getMessage());
                continue;
            }
            onDataReceived(packet);
        }
    }

    private void onDataReceived(DatagramPacket packet) {
        String data = new String(packet.getData(), 0, packet.getLength());
        logger.info(String.format("[%s] UDP packet received: %s", packet.getSocketAddress().toString(), data));
        WofPacket wofPacket = new Gson().fromJson(data, WofPacket.class);
        if (!clients.containsKey(packet.getSocketAddress().toString())) {
            logger.info("New UDP client " + packet.getSocketAddress().toString());
            clients.put(packet.getSocketAddress().toString(), packet);
        }

        if (wofPacket.isIdentification()) {
            // this is the only packet we care about for now
            logger.info("Identifying UDP client...");
            WofServer.clientIdentified(wofPacket, packet);
        }

    }

    private void watchdog() {
        while (running) {
            try {
                Thread.sleep(5000);
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
            clients.forEach((addr, datagramPacket) -> {
                logger.info("sending upd packet");
                String data = "{\"ping\":\"pong\"}";
                DatagramPacket packet = new DatagramPacket(data.getBytes(), data.length(), datagramPacket.getSocketAddress());
                WofServer.sendUdpPacket(packet);
            });
        }
    }

    public void send(DatagramPacket dp) throws IOException {
        socket.send(dp);
    }
}
