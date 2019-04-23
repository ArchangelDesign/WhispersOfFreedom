package com.whispersoffreedom.server.exception;

public class ServerFullException extends RuntimeException {
    public ServerFullException() {
        super("Server is full.");
    }
}
