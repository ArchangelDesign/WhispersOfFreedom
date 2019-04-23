package com.whispersoffreedom.client;

import org.json.JSONObject;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.Socket;

public class Client {
    public static final String SERVER_URL = "http://localhost:8080";
    public static final String TCP_SERVER = "http://localhost:8081";
    Logger logger = LoggerFactory.getLogger(Client.class);
    Socket connection;
    String currentSessionToken;
    ApiClient apiClient = new ApiClient(SERVER_URL);
    private boolean isSynchornized = false;
    private boolean connected = false;
    private Thread connectionThread;
    boolean inBattle = false;

    public static void main(String[] args) throws IOException, InterruptedException {
        Client client = new Client();
        client.logger.info("Starting client...");
        client.runClient();
    }

    private void runClient() throws IOException, InterruptedException {
        Thread.sleep(100);
        System.out.print("username: ");
        String username = readFromConsole();
        logger.info("connecting to " + SERVER_URL);
        String sessionToken = apiClient.enterServer(username);
        currentSessionToken = sessionToken;
        connected = true;
        startTcpConnection(sessionToken);
        logger.info("session token: " + sessionToken);
        String cmd;
        while (!(cmd = readFromConsole()).equals("exit")) {
            handleCommand(cmd);
        }
        System.out.print("press enter to exit.");
        readFromConsole();
        System.exit(0);
    }

    private void handleCommand(String cmd) throws IOException {
        switch (cmd) {
            case "start":
                if (inBattle) {
                    System.out.println("already in battle. starting the battle.");

                    return;
                }
                logger.info("starting battle...");
                String battleToken = apiClient.startBattle(currentSessionToken);
                logger.info("Battle started. Token: " + battleToken);
                inBattle = true;
                break;
            case "leave":
                if (!inBattle)
                    return;
                logger.info("leaving the battle...");
                apiClient.leaveBattle(currentSessionToken);
                inBattle = false;
                break;
            case "join":
                if (inBattle) {
                    System.out.println("Already in battle");
                    return;
                }
                System.out.print("token of battle to join: ");
                String t = readFromConsole();
                logger.info("entering existing battle...");
                apiClient.enterBattle(t, currentSessionToken);
                break;
        }
    }

    private String readFromConsole() throws IOException {
        InputStreamReader reader = new InputStreamReader(System.in);
        BufferedReader bufferedReader = new BufferedReader(reader);
        return bufferedReader.readLine();
    }

    private void startTcpConnection(String sessionToken) throws IOException {
        logger.info("starting tcp connection...");
        connection = new Socket("localhost", 8081);
        connectionThread = new Thread(this::tcpConnectionHandler);
        connectionThread.start();
    }

    private void tcpConnectionHandler() {
        try {
            logger.info("sending identification...");
            identifyYourself();
            BufferedReader in = new BufferedReader(
                    new InputStreamReader(connection.getInputStream()));
            String inputLine;
            while ((inputLine = in.readLine()) != null) {
                onMessageReceived(inputLine);
            }
        } catch (IOException | InterruptedException e) {
            logger.error(e.getMessage());
        }
    }

    private void identifyYourself() throws IOException, InterruptedException {
        Thread.sleep(300);
        PrintWriter writer = new PrintWriter(connection.getOutputStream(), true);
        JSONObject entity = new JSONObject();
        entity.put("clientId", currentSessionToken);
        entity.put("command", "identification");
        writer.println(entity.toString());
    }

    private void sendCommand(String cmd, String arg1) throws IOException {
        PrintWriter writer = new PrintWriter(connection.getOutputStream(), true);
        JSONObject entity = new JSONObject();
        entity.put("clientId", currentSessionToken);
        entity.put("command", cmd);
        entity.put("arg1", arg1);
        writer.println(entity.toString());
    }

    private void onMessageReceived(String message) {
        JSONObject command = new JSONObject(message);
        switch (command.getString("command")) {
            case "ack":
                return;
            case "broadcast":
                System.out.println(command.getString("arg1"));
                break;
            case "ping":
                System.out.print('.');
                break;
            case "welcome":
                // welcome message should arrive as soon as we connect via TCP
                // and identify ourselves to associate REST session with TCP socket
                isSynchornized = true;
                logger.info("Client is now synchronized.");
            default:
                logger.info("Message from server: " + message);
        }

    }
}
