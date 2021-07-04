package com.whispersoffreedom.server.command;

import com.whispersoffreedom.server.TcpConnection;
import com.whispersoffreedom.server.packet.WofPacket;
import com.whispersoffreedom.server.packet.WofPacketSimple;
import com.whispersoffreedom.server.WofServer;

public class Identification extends WofCommand {

    public Identification(TcpConnection connection, WofPacket packet) {
        super(connection, packet);
    }

    @Override
    public void execute() {
        logger.info("Identifying client... " + packet.getClientId());
        WofServer.clientIdentifiedTcp(connection, packet);
        super.connection.sendPacket(new WofPacketSimple(super.packet.getClientId(), "welcome"));
    }
}
