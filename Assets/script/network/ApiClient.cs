using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

public class ServerStats
{
    public int clientCount;
    public int battleCount;
}

public class ApiClient
{

    // MAX wait time for response from REST server
    public const long TIMEOUT = 30;

    private ServerState serverState = ServerState.READY;

    private string URL = GlobalConfig.API_URL;

    private string TcpServerUrl = GlobalConfig.BATTLE_SERVER_ADDRESS;

    private int TcpServerPort = GlobalConfig.BATTLE_SERVER_PORT;

    private string sessionToken = null;

    private string lastResponse;

    private string lastError;

    private long lastResponseCode;

    private string tcpLastResponse;

    private Battle[] battles;

    private bool loggedIn = false;

    private bool tcpConnected = false;

    private bool isConnecting = false;

    private bool inBattle = false;

    private string currentBattleId;

    private TcpClient tcpClient;

    private Thread receiveThread;

    private static ApiClient instance;

    private long lastPing = DateTime.Now.Ticks;

    private ulong lastTx = 0;

    private ulong lastRx = 0;

    private ulong totalTx = 0;

    private ulong totalRx = 0;

    private ServerStats serverStats = new ServerStats();

    public delegate void EnterServerCallback(bool success);

    public static ApiClient getInstance()
    {
        if (instance == null)
            instance = new ApiClient();

        return instance;
    }

    public void EnterServerAsync(string username, string password, EnterServerCallback callback)
    {
        Thread t = new Thread(() => EnterServer(username, password, callback));
        t.Start();
    }

    public bool EnterServer(string username, string password, EnterServerCallback callback)
    {
        Debug.Log("Entering server...");
        if (loggedIn)
        {
            if (callback != null)
                callback(false);
            return false;
        }
        EnterServerRequest request = new EnterServerRequest(username, password);
        lastResponse = null;
        string response;
        try
        {
           response = SendPostRequest("/user/enter", JsonUtility.ToJson(request));
        } catch(Exception se)
        {
            if (callback != null)
                callback(false);
            Debug.Log("Socket exception: " + se.Message);
            return false;
        }
        GenericResponse genericResponse = JsonUtility.FromJson<GenericResponse>(response);
        if (genericResponse == null)
        {
            Debug.LogError("Cannot connect to server.");
            lastError = "Cannot connect to server. Invalid response.";
            if (callback != null)
                callback(false);
            return false;
        }
        if (genericResponse.sessionToken == null)
        {
            lastError = "Cannot connect to server. No session token received.";
            if (callback != null)
                callback(false);
            return false;
        }
        if (genericResponse.sessionToken != null)
        {
            Debug.Log("received session token");
            sessionToken = genericResponse.sessionToken;
            loggedIn = true;
            InitiateTcpConnection();
            InitiateUdpConnection();
            if (callback != null)
                callback(true);
            return true;
        }

        lastError = "Unexpected outcome.";
        if (callback != null)
            callback(false);
        return false;
    }

    public void LeaveServer()
    {
        Debug.Log("Dicsonnecting from server.");
        SendPostRequest("/user/disconnect", "{\"empy\":\"empty\"}");
        DiconnectTcpClient();
        DisconnectUdpClient();
        loggedIn = false;
        sessionToken = null;
    }

    private void DiconnectTcpClient()
    {
        tcpClient.Dispose();
        tcpConnected = false;
    }

    private void DisconnectUdpClient()
    {
        // Should be disconnected after TCP goes offline
        // @TODO
    }

    public String GetClientId()
    {
        return sessionToken;
    }

    private void InitiateTcpConnection()
    {
        if (isConnecting)
        {
            Debug.Log("Connection attempt in progress...");
            return;
        }
        Debug.Log("initializing TCP connection.");
        try
        {
            receiveThread = new Thread(new ThreadStart(TcpDataHandler));
            receiveThread.IsBackground = true;
            receiveThread.Start();
            Debug.Log("TCP thread started.");
        } catch (Exception e)
        {
            Debug.LogError("TCP ERROR: " + e.Message);
        }
    }

    private void InitiateUdpConnection()
    {
        WofUdpClient.GetInstance().StartClient();
    }

    private void IdentifyYourself()
    {
        if (!loggedIn)
            return;
        // @TODO: TCP connection here
    }

    public Battle[] ListBattles()
    {
        lastResponse = null;
        string response = SendGetRequest("/user/battle-list");
        if (response == null)
            return battles;

        battles = JsonHelper.FromJson<Battle>(response);
   
        return battles;
    }

    public void StartBattle() {
        if (!inBattle)
            return;
    }

    public void RenameBattle(string newName) {
        if (!inBattle)
            return;
      }

    public void leaveBattle() {
        if (!inBattle)
            return;
    }

    public void JoinBattle(string battleId)
    {
        if (inBattle)
            return;
        string body = new EnterBattleRequest(battleId).ToJson();
        string response = SendPostRequest("/user/enter-battle", body);

    }

    public void CreateBattle() {
        if (inBattle)
            return;
    }

    public void TerminateBattle() { }

    public bool IsLoggedIn()
    {
        return loggedIn;
    }

    public bool IsTcpClientConnected()
    {
        return tcpConnected;
    }

    public Battle[] GetBattles()
    {
        return battles;
    }

    private string SendPostRequest(string uri, string body)
    {
        return SendRequest(uri, HttpMethod.Post, body);
            
    }

    private string SendGetRequest(string uri)
    {
        return SendRequest(uri, HttpMethod.Get, "");
    }

    string SendRequest(string uri, HttpMethod method, string body)
    {
        HttpClient httpClient = new HttpClient();
        Debug.Log("[" + method + "] " + uri);
        serverState = ServerState.TX;

        httpClient.BaseAddress = new Uri(URL);
        httpClient.Timeout = new TimeSpan(0, 0, 6);
        httpClient.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        if (sessionToken != null)
            httpClient.DefaultRequestHeaders.Add("session-token", sessionToken);

        HttpRequestMessage request = new HttpRequestMessage(method, uri);
        if (method != HttpMethod.Get)
            request.Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
        if (sessionToken != null && method != HttpMethod.Get)
            request.Content.Headers.Add("session-token", sessionToken);


        HttpResponseMessage response = httpClient.SendAsync(request).Result;

       
        lastTx = (ulong)body.Length;
        lastRx = (ulong)response.Content.ReadAsByteArrayAsync().Result.Length;
        totalTx += lastTx;
        totalRx += lastRx;
        // @TODO
        lastResponseCode = response.IsSuccessStatusCode ? 200 : 500;

        if (!response.IsSuccessStatusCode)
            Debug.LogError("Client or server error.");
        
        serverState = ServerState.RX;
        lastResponse = response.Content.ReadAsStringAsync().Result;
        serverState = ServerState.READY;
        return lastResponse;
    }

    string SendRequest(string uri, HttpMethod method)
    {
        return SendRequest(uri, method, "");
    }

    private string BytesToString(byte[] bytes)
    {
        return Encoding.Decode(bytes);
    }

    private void OnDataReceived(string data) {
        lastResponse = data;
    }

    private void OnTcpDataReceived(string data)
    {
        Debug.Log("TCP data received: " + data);
        WofCommand cmd = JsonUtility.FromJson<WofCommand>(data);

        switch (cmd.command.ToLower())
        {
            case "ping":
                lastPing = DateTime.Now.Ticks;
                SendPongCommand();
                break;

            case "ack":
                Debug.Log("ACK");
                break;

            case "welcome":
                Notification.getInstance().InfoAsync("Connected to server.");
                break;

            default:
                Debug.LogError("Unhandled command from server: " + cmd.command);
                break;
        }
    }

    public long SecondsSinceLastPing()
    {
        return (DateTime.Now.Ticks - lastPing) / 10000000;
    }

    public string GetTotalRx()
    {
        return totalRx.ToString();
    }

    public string GetTotalTx()
    {
        return totalTx.ToString();
    }

    public void reportStats(int clientCount, int battleCount)
    {
        serverStats.battleCount = battleCount;
        serverStats.clientCount = clientCount;
    }

    public ServerStats getStats()
    {
        return serverStats;
    }

    private void TcpDataHandler()
    {
        isConnecting = true;
        Notification.getInstance().InfoAsync("Connecting to server...");
        tcpClient = new TcpClient(TcpServerUrl, TcpServerPort);
        Debug.Log("Created TCPClient object.");
        tcpConnected = true;
        isConnecting = false;
        Byte[] buffer = new byte[1024];
        tcpConnected = true;

        // now we need to associate TCP connection with session
        Thread.Sleep(200);
        IdentificationCommand cmd = new IdentificationCommand(sessionToken);
        TcpSendData(JsonUtility.ToJson(cmd));

        while (tcpClient.Connected)
        {
            NetworkStream stream = tcpClient.GetStream();
            int length = 0;
            while ((length = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                byte[] receivedBytes = new byte[length];
                Array.Copy(buffer, 0, receivedBytes, 0, length);
                tcpLastResponse = Encoding.Decode(receivedBytes);
                OnTcpDataReceived(tcpLastResponse);
            }
        }
        Debug.LogError("Server disconnected");
    }

    private void TcpSendData(string data)
    {
        if (!tcpConnected)
        {
            Debug.LogError("Trying to send TCP packet when client is not connected.");
            return;
        }

        NetworkStream stream = tcpClient.GetStream();

        if (!stream.CanWrite)
        {
            Debug.LogError("Cannot write to TCP stream.");
            return;
        }

        StreamWriter writer = new StreamWriter(stream);
        byte[] rawData = Encoding.Encode(data);
        writer.WriteLine(data);
        writer.Flush();
    }

    private void OnError(string error)
    {
        Debug.LogError(error);
    }

    private void SendPongCommand()
    {
        TcpSendData(new PongCommand(sessionToken).ToJson());
    }

    public void OnDestroy()
    {
        Debug.Log("ApiClient is terminating.");
        if (tcpClient != null)
            tcpClient.Dispose();
        //receiveThread.Abort();
    }

    public string GetLastError()
    {
        return lastError;
    }
}
