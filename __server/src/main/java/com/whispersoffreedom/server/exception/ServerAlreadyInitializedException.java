package com.whispersoffreedom.server.exception;

public class ServerAlreadyInitializedException extends RuntimeException {
    public ServerAlreadyInitializedException() {
        super("Server already initialized.");
    }
}
