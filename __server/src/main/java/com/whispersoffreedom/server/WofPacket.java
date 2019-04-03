package com.whispersoffreedom.server;

import lombok.Getter;
import lombok.Setter;
import lombok.experimental.Accessors;

@Getter
@Setter
@Accessors(chain = true)
public class WofPacket {
    private String clientId;
    private String command;
    private String arg1;
    private String memo;
}
