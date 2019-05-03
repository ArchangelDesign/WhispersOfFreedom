package com.whispersoffreedom.server;

import com.google.gson.Gson;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import lombok.experimental.Accessors;

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

    public String toJson() {
        Gson g = new Gson();

        return g.toJson(this);
    }

    public boolean isIdentification() {
        return command.equalsIgnoreCase("identification");
    }
}
