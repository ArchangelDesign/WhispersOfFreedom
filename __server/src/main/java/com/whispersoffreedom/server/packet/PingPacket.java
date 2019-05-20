package com.whispersoffreedom.server.packet;

public class PingPacket extends WofPacket {
    public PingPacket() {
        command = "ping";
    }
}
