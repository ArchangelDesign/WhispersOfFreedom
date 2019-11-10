package com.whispersoffreedom.server.command;

import com.whispersoffreedom.server.TcpConnection;
import com.whispersoffreedom.server.packet.PongPacket;
import com.whispersoffreedom.server.packet.WofPacket;

public class PingPong extends WofCommand {

    private String type;

    public PingPong(TcpConnection connection, WofPacket packet, String type) {
        super(connection, packet);
        this.type = type;
    }

    @Override
    public void execute() {
        if (type.equalsIgnoreCase("ping")) {
            super.connection.sendPacket(new PongPacket());
        }
    }
}
