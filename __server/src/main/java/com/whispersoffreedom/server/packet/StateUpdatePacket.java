package com.whispersoffreedom.server.packet;

import com.whispersoffreedom.server.Client;

import java.time.Instant;

public class StateUpdatePacket extends WofPacket {

    public StateUpdatePacket(Client client) {
        command = "state_update";
        params.put("inBattle", client.isInBattle() ? "true" : "false");
        params.put("connected", client.getConnected().toString());
        params.put("currentTime", Instant.now().toString());
        if (!client.isInBattle())
            return;

        params.put("battleId", client.getCurrentBattle().getBattleId());
        params.put("battleName", client.getCurrentBattle().getName());
        params.put("battleState", client.getCurrentBattle().getStatus().toString());
    }
}
