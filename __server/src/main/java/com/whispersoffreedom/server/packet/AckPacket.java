package com.whispersoffreedom.server.packet;

public class AckPacket extends WofPacket {
    public AckPacket() {
        command = "ack";
    }
}
