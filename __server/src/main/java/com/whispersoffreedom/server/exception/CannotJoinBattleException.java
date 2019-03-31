package com.whispersoffreedom.server.exception;

public class CannotJoinBattleException extends RuntimeException {
    public CannotJoinBattleException() {
        super("Cannot join battle. Already started or lobby removed.");
    }
}
