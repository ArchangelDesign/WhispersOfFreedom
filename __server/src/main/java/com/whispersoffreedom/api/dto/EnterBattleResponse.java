package com.whispersoffreedom.api.dto;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.experimental.Accessors;

@NoArgsConstructor
@AllArgsConstructor
@Getter
@Accessors(chain = true)
public class EnterBattleResponse {
    private boolean success = false;
    private String battleToken;
}
