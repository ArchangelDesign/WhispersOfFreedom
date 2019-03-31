package com.whispersoffreedom.server;

import com.whispersoffreedom.server.exception.BattleAlreadyStarted;
import lombok.Getter;

import java.util.HashMap;
import java.util.UUID;

public class Battle {
    @Getter
    private BattleStatus status = BattleStatus.INITIALIZING;

    @Getter
    private String battleId = UUID.randomUUID().toString();

    @Getter
    private HashMap<String, Client> clients = new HashMap<>();

    @Getter
    private String name = "Auto-generated from " + battleId;

    public Battle() {
        new Thread(() -> {
            try {
                Thread.sleep(1000);
            } catch (InterruptedException e) {
            }
            status = BattleStatus.LOBBY;
        }).start();
    }

    public void broadcast(String message) {
        if (!status.equals(BattleStatus.LOBBY) && !status.equals(BattleStatus.IN_PROGRESS))
            return;

        clients.forEach((uuid, client) -> client.broadcast(message));
    }

    public void rename(Client c, String newName) {
        if (!status.equals(BattleStatus.LOBBY))
            throw new BattleAlreadyStarted();
        broadcast(String.format(
                "User %s is changing the name of the lobby to %s",
                c.getUsername(), newName
        ));

        name = newName;
    }

    public void clientEnters(Client c) {
        broadcast(String.format(
                "Client connected: %s", c.getUsername()
        ));
        clients.put(c.getId().toString(), c);
    }

    public void clientLeaves(Client c) {
        broadcast(String.format(
                "Client disconnected: %s", c.getUsername()
        ));
        clients.remove(c.getId().toString());
    }
}
