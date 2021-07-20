package com.whispersoffreedom.server.packet;

import com.whispersoffreedom.server.Client;
import com.whispersoffreedom.server.WofServer;

import java.time.Instant;

public class StateUpdatePacket extends WofPacket {

    public StateUpdatePacket(Client client) {
        command = "state_update";
        parameters.put("inBattle", client.isInBattle() ? "true" : "false");
        parameters.put("connected", client.getConnected().toString());
        parameters.put("currentTime", String.valueOf(Instant.now().getEpochSecond()));
        parameters.put("currentTimeFraction", String.valueOf(Instant.now().getNano()));
        parameters.put("clientCount", String.valueOf(WofServer.getClientCount()));
        parameters.put("battleCount", String.valueOf(WofServer.getBattleCount()));
        if (!client.isInBattle())
            return;

        parameters.put("battleId", client.getCurrentBattle().getBattleId());
        parameters.put("battleName", client.getCurrentBattle().getName());
        parameters.put("battleState", client.getCurrentBattle().getStatus().toString());
    }
}
