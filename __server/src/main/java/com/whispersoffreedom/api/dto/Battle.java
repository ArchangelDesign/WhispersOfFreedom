package com.whispersoffreedom.api.dto;

import lombok.AllArgsConstructor;
import lombok.Getter;

@Getter
@AllArgsConstructor
public class Battle {
    private Integer clients;
    private String name;
    private String battleId;
}
