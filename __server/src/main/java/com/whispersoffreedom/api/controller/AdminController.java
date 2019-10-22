package com.whispersoffreedom.api.controller;

import com.whispersoffreedom.server.Client;
import com.whispersoffreedom.server.WofServer;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestHeader;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;

@RestController
@RequestMapping("/admin")
public class AdminController {

    @GetMapping("/get-clients")
    public List<Client> getClientList(
            @RequestHeader(name = "session-token") String sessionToken
    ) {
        // @TODO: requires admin account
        if (WofServer.getClientCount() == 0)
            return null;

        return WofServer.getAllClients();
    }
}
