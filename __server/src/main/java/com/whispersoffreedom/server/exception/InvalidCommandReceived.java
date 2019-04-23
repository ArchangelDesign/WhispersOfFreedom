package com.whispersoffreedom.server.exception;

public class InvalidCommandReceived extends RuntimeException {
    public InvalidCommandReceived() {
        super("Invalid command received.");
    }
}
