package com.whispersoffreedom.server;

public class WofPacketBroadcast extends WofPacket {

    public WofPacketBroadcast(String clientId, String message) {
        this.clientId = clientId;
        this.arg1 = message;
        this.command = "broadcast";
    }
}
