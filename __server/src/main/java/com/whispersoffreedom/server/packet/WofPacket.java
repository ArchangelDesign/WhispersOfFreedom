package com.whispersoffreedom.server.packet;

import com.google.gson.Gson;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import lombok.experimental.Accessors;

import java.time.Instant;
import java.util.ArrayList;
import java.util.HashMap;

@Getter
@Setter
@Accessors(chain = true)
@NoArgsConstructor
@AllArgsConstructor
public class WofPacket {
    protected String clientId;
    protected String command;
    protected String arg1;
    protected String memo;
    protected HashMap<String, String> parameters = new HashMap<>();
    // Packets are also used by UDP server
    protected Instant timestamp = Instant.now();

    public String toJson() {
        Gson g = new Gson();

        return g.toJson(this);
    }

    public boolean isIdentification() {
        return command.equalsIgnoreCase("identification");
    }
}
