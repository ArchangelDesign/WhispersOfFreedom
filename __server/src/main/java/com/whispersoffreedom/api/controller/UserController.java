package com.whispersoffreedom.api.controller;

import com.whispersoffreedom.api.dto.*;
import com.whispersoffreedom.server.Battle;
import com.whispersoffreedom.server.Client;
import com.whispersoffreedom.server.WofServer;
import com.whispersoffreedom.server.exception.ServerFullException;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.*;

import java.util.ArrayList;
import java.util.List;

@RestController
@RequestMapping("/user")
public class UserController {

    Logger logger = LoggerFactory.getLogger(UserController.class);

    @GetMapping({"/health", "/healthCheck", "/health-check"})
    @ResponseStatus(code = HttpStatus.OK)
    public void healthCheck() {
    }

    @PostMapping("/enter")
    public WofSession enterServer(@RequestBody StartSessionRequest request) {
        if (WofServer.isFull())
            throw new ServerFullException();
        Client client = WofServer.registerClient(request.getUsername());
        WofServer.enterServer(request.getUsername());
        return new WofSession(client.getId().toString());
    }

    @PostMapping("/enter-battle")
    public EnterBattleResponse enterBattle(
            @RequestHeader(name = "session-token") String sessionToken,
            @RequestBody EnterBattleRequest request) {

        Battle b = WofServer.getBattle(request.getBattleId());
        Client c = WofServer.getClient(sessionToken);
        WofServer.clientEntersBattle(c, b);

        return new EnterBattleResponse(true, b.getBattleId());
    }

    @PostMapping("/start-battle")
    public EnterBattleResponse startBattle(@RequestHeader(name = "session-token") String sessionToken) {
        return new EnterBattleResponse(true, WofServer.initializeBattle(sessionToken).getBattleId());
    }

    @PostMapping("/leave-battle")
    public void leaveBattle(@RequestHeader(name = "session-token") String sessionToken) {
        Client c = WofServer.getClient(sessionToken);
        WofServer.clientLeavesBattle(c);
    }

    @GetMapping("/battle-list")
    public List<com.whispersoffreedom.api.dto.Battle> listOpenBattles() {
        ArrayList<com.whispersoffreedom.api.dto.Battle> result = new ArrayList<>();
        for (Battle b : WofServer.getOpenLobbies())
            result.add(new com.whispersoffreedom.api.dto.Battle(
                    b.getClients().size(), b.getName(), b.getBattleId()));

        return result;
    }

    @PostMapping("/rename-battle")
    public void renameBattle(
            @RequestHeader(name = "session-token") String sessionToken,
            @RequestBody RenameBattleRequest request) {
        WofServer.renameBattle(WofServer.getClient(sessionToken), request.getName());
    }

}
