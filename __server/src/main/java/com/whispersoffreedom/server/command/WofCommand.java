package com.whispersoffreedom.server.command;

import com.whispersoffreedom.server.TcpConnection;
import com.whispersoffreedom.server.packet.WofPacket;
import com.whispersoffreedom.server.exception.InvalidCommandReceived;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public abstract class WofCommand {

    protected TcpConnection connection;

    protected String command;

    protected String arg1;

    protected WofPacket packet;

    protected Logger logger = LoggerFactory.getLogger(WofCommand.class);

    public WofCommand(TcpConnection connection, WofPacket packet) {
        this.connection = connection;
        this.command = packet.getCommand();
        this.arg1 = packet.getArg1();
        this.packet = packet;
    }

    public static WofCommand build(TcpConnection connection, WofPacket packet) {
        switch (packet.getCommand().toLowerCase()) {
            case "identification": return new Identification(connection, packet);
            case "start": return new StartBattle(connection, packet);
            case "terminate": return new TerminateBattle(connection, packet);
            case "pong": return new PingPong(connection, packet, "pong");
            case "ping": return new PingPong(connection, packet, "ping");
        }

        throw new InvalidCommandReceived();
    }

    public abstract void execute();
}
