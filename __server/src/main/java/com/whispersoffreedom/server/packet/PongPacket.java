package com.whispersoffreedom.server.packet;

public class PongPacket extends WofPacket {
    public PongPacket() {
        command = "pong";
    }
}
