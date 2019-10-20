package com.whispersoffreedom.server.exception;

import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.ResponseStatus;

@ResponseStatus(code = HttpStatus.BAD_REQUEST)
public class UserAlreadyConnectedException extends Exception {
    public UserAlreadyConnectedException() {
        super("User already connected.");
    }
}
