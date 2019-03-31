package com.whispersoffreedom.server.exception;

import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.ResponseStatus;

@ResponseStatus(code = HttpStatus.NOT_FOUND)
public class BattleNotFoundException extends RuntimeException {

    public BattleNotFoundException() {
        super("Battle not found");
    }
}
