package com.whispersoffreedom.server.command;

import com.whispersoffreedom.server.BattleStatus;
import com.whispersoffreedom.server.TcpConnection;
import com.whispersoffreedom.server.packet.WofPacket;
import com.whispersoffreedom.server.exception.NotInBattleException;

public class StartBattle extends WofCommand {

    public StartBattle(TcpConnection connection, WofPacket packet) {
        super(connection, packet);
    }

    @Override
    public void execute() {
        if (connection.getClient() == null)
            throw new RuntimeException("Connection is not identified. Cannot start the battle.");
        if (!connection.getClient().isInBattle())
            throw new NotInBattleException();
        if (!connection.getClient().getCurrentBattle().isHost(connection.getClient()))
            throw new RuntimeException("Not your battle.");
        if (!connection.getClient().getCurrentBattle().getStatus().equals(BattleStatus.LOBBY))
            throw new RuntimeException("Invalid battle status");
        logger.info(connection.getClient().getUsername() + " is starting the battle...");
        connection.getClient().getCurrentBattle().startBattle(connection.getClient());
    }
}
