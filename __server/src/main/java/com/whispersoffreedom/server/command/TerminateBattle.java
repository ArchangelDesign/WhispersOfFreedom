package com.whispersoffreedom.server.command;

import com.whispersoffreedom.server.TcpConnection;
import com.whispersoffreedom.server.WofPacket;

public class TerminateBattle extends WofCommand {

    public TerminateBattle(TcpConnection connection, WofPacket packet) {
        super(connection, packet);
    }

    @Override
    public void execute() {
        if (!connection.getClient().isInBattle())
            return;
        if (!connection.getClient().getCurrentBattle().isHost(connection.getClient()))
            return;
        connection.getClient().getCurrentBattle().terminateBattle(connection.getClient());
    }
}
