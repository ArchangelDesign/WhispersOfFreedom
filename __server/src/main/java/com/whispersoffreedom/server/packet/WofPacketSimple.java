package com.whispersoffreedom.server.packet;

public class WofPacketSimple extends WofPacket {
    public WofPacketSimple(String clientId, String command) {
        this.command = command;
        this.clientId = clientId;
    }
}
