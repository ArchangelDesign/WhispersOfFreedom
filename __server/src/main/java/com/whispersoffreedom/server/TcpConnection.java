package com.whispersoffreedom.server;

import lombok.Getter;

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

    public void noticeTransmission() {
        lastTransmission = Instant.now();
    }

    public TcpConnection(UUID id, Socket socket) {
        this.socket = socket;
        this.id = id;
        try {
            outputWriter = new PrintWriter(socket.getOutputStream(), true);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public void sendPacket(WofPacket packet) {
        lastWrite = Instant.now();
    }

    public void packetReceived(WofPacket pck) {
        lastPacket = pck;
        lastRead = Instant.now();
    }

}
