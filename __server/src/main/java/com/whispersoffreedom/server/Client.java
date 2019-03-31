package com.whispersoffreedom.server;

import lombok.Getter;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.util.UUID;

public class Client {

    @Getter
    private UUID id = UUID.randomUUID();

    @Getter
    private String username;

    @Getter
    private boolean inBattle = false;

    @Getter
    private Battle currentBattle = null;

    @Getter
    private Battle previousBattle = null;

    Logger logger = LoggerFactory.getLogger(String.format("Client [%s]", id));

    public Client(String newUsername) {
        username = newUsername;
    }

    public void enterBattle(Battle battle) {
        currentBattle = battle;
        inBattle = true;
    }

    public void leaveBattle() {
        previousBattle = currentBattle;
        currentBattle = null;
        inBattle = false;
    }

    public void broadcast(String message) {
        logger.info("Broadcasting message.");
    }
}
