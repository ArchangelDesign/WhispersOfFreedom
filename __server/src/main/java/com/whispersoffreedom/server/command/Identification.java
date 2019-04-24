package com.whispersoffreedom.server.command;

import com.whispersoffreedom.server.TcpConnection;
import com.whispersoffreedom.server.WofPacket;
import com.whispersoffreedom.server.WofPacketSimple;
import com.whispersoffreedom.server.WofServer;

public class Identification extends WofCommand {

    public Identification(TcpConnection connection, WofPacket packet) {
        super(connection, packet);
    }

    @Override
    public void execute() {
        logger.info("Identifying client... " + packet.getClientId());
        WofServer.clientIdentified(connection, packet);
        super.connection.sendPacket(new WofPacketSimple(super.packet.getClientId(), "welcome"));
    }
}
