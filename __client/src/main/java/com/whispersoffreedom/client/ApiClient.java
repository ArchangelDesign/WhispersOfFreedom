package com.whispersoffreedom.client;

import okhttp3.*;
import okio.BufferedSink;
import org.json.JSONArray;
import org.json.JSONObject;

import java.io.IOException;

public class ApiClient {

    String serverUrl;

    public ApiClient(String url) {
        serverUrl = url;
    }

    public String enterServer(String username) throws IOException {
        OkHttpClient client = new OkHttpClient();

        MediaType mediaType = MediaType.parse("application/json");
        RequestBody body = RequestBody.create(mediaType, "{\"username\": \""+ username+"\"}");
        Request request = new Request.Builder()
                .url(serverUrl + "/user/enter")
                .post(body)
                .addHeader("Content-Type", "application/json")
                .addHeader("cache-control", "no-cache")
                .build();

        Response response = client.newCall(request).execute();
        if (response.code() > 201)
            throw new RuntimeException("Connection failed. " + response.message());
        return new JSONObject(response.body().string()).getString("sessionToken");
    }

    public String startBattle(String sessionToken) throws IOException {
        OkHttpClient client = new OkHttpClient();

        Request request = new Request.Builder()
                .url(serverUrl + "/user/start-battle")
                .post(new RequestBody() {
                    @Override
                    public MediaType contentType() {
                        return null;
                    }

                    @Override
                    public void writeTo(BufferedSink sink) throws IOException {

                    }
                })
                .addHeader("session-token", sessionToken)
                .addHeader("Content-Type", "application/json")
                .addHeader("cache-control", "no-cache")
                .build();

        Response response = client.newCall(request).execute();
        if (response.code() > 201)
            throw new RuntimeException("Connection failed. " + response.message());
        return new JSONObject(response.body().string()).getString("battleToken");
    }

    public void leaveBattle(String sessionToken) throws IOException {
        OkHttpClient client = new OkHttpClient();

        Request request = new Request.Builder()
                .url(serverUrl + "/user/leave-battle")
                .post(new RequestBody() {
                    @Override
                    public MediaType contentType() {
                        return null;
                    }

                    @Override
                    public void writeTo(BufferedSink sink) throws IOException {

                    }
                })
                .addHeader("session-token", sessionToken)
                .addHeader("cache-control", "no-cache")
                .build();

        Response response = client.newCall(request).execute();
        if (response.code() > 201)
            throw new RuntimeException("Connection failed. " + response.message());
    }

    public void enterBattle(String battleToken, String sessionToken) throws IOException {
        OkHttpClient client = new OkHttpClient();

        MediaType mediaType = MediaType.parse("application/json");
        RequestBody body = RequestBody.create(mediaType, "{\"battleId\": \"" + battleToken + "\"}");
        Request request = new Request.Builder()
                .url(serverUrl + "/user/enter-battle")
                .post(body)
                .addHeader("session-token", sessionToken)
                .addHeader("Content-Type", "application/json")
                .addHeader("cache-control", "no-cache")
                .build();

        Response response = client.newCall(request).execute();
    }

    public JSONArray getBattleList(String sessionToken) throws IOException {
        OkHttpClient client = new OkHttpClient();

        Request request = new Request.Builder()
                .url(serverUrl + "/user/battle-list")
                .get()
                .addHeader("session-token", sessionToken)
                .addHeader("cache-control", "no-cache")
                .build();

        Response response = client.newCall(request).execute();
        return new JSONArray(response.body().string());
    }
}
