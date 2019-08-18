package com.whispersoffreedom.api.dto;

import lombok.Getter;

@Getter
public class StartSessionRequest {
    private String username;
    private String password;
}
