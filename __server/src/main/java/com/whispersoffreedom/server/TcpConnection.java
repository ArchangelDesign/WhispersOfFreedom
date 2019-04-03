package com.whispersoffreedom.server;

import com.google.gson.Gson;
import com.google.gson.JsonSyntaxException;
import lombok.Getter;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.IOException;
import java.io.PrintWriter;
import java.net.Socket;
import java.time.Instant;
import java.util.UUID;

@Getter
public class TcpConnection {

    private Socket socket;

    private UUID id;

    private Instant created = Instant.now();

    private Instant lastTransmission = Instant.now();

    private Instant lastRead;

    private Instant lastWrite;

    private WofPacket lastPacket;

    private PrintWriter outputWriter;

    private Logger logger;

    public void noticeTransmission() {
        lastTransmission = Instant.now();
    }

    public TcpConnection(UUID id, Socket socket) {
        this.socket = socket;
        this.id = id;
        logger = LoggerFactory.getLogger(String.format("TCP Connection [%s]", socket.getRemoteSocketAddress()));
        try {
            outputWriter = new PrintWriter(socket.getOutputStream(), true);
            logger.info("Connection established [" + id + "]");
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public void sendPacket(WofPacket packet) {
        lastWrite = Instant.now();
        Gson g = new Gson();
        String toSend = g.toJson(packet);
        outputWriter.println(toSend);
    }

    public void write(byte[] data) throws IOException {
        socket.getOutputStream().write(data);
    }

    public String readPacket() {
        return "";
    }

    public void packetReceived(WofPacket pck) {
        lastPacket = pck;
        lastRead = Instant.now();
        logger.info("Packet received. Command: " + pck.getCommand());
    }

    public void rawPacketReceived(String packet) {
        Gson g = new Gson();
        WofPacket wofPacket;
        try {
            wofPacket = g.fromJson(packet, WofPacket.class);
        } catch (JsonSyntaxException syntaxException) {
            logger.error("Invalid JSON received: " + packet);
            return;
        }
        if (wofPacket == null) {
            logger.error("Packet deserialization failed: " + packet);
            return;
        }
        packetReceived(wofPacket);
    }
}
