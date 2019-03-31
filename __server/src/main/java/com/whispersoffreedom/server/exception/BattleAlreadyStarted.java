package com.whispersoffreedom.server.exception;

import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.ResponseStatus;

@ResponseStatus(code = HttpStatus.BAD_REQUEST)
public class BattleAlreadyStarted extends RuntimeException {
    public BattleAlreadyStarted() {
        super("Battle has already started.");
    }
}
