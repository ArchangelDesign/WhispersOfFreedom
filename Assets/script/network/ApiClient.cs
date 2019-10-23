using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    public static ApiClient getInstance()
    {
        if (instance == null)
            instance = new ApiClient();

        return instance;
    }

    public bool EnterServer(string username, string password)
    {
        if (loggedIn)
            return false;
        EnterServerRequest request = new EnterServerRequest(username, password);
        lastResponse = null;

        string response = SendPostRequest("/user/enter", JsonUtility.ToJson(request));
        GenericResponse genericResponse = JsonUtility.FromJson<GenericResponse>(response);
        if (genericResponse == null)
        {
            Debug.LogError("Cannot connect to server.");
            lastError = "Cannot connect to server. Invalid response.";
            return false;
        }
        if (genericResponse.sessionToken == null)
        {
            lastError = "Cannot connect to server. No session token received.";
            return false;
        }
        if (genericResponse.sessionToken != null)
        {
            Debug.Log("received session token");
            sessionToken = genericResponse.sessionToken;
            loggedIn = true;
            InitiateTcpConnection();
            InitiateUdpConnection();
            return true;
        }

        lastError = "Unexpected outcome.";
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
        Debug.Log("initializing TCP connection.");
        try
        {
            tcpClient = new TcpClient(TcpServerUrl, TcpServerPort);
            receiveThread = new Thread(new ThreadStart(TcpDataHandler));
            receiveThread.IsBackground = true;
            receiveThread.Start();
            tcpConnected = true;
            Debug.Log("TCP thread started.");
            // now we need to associate TCP connection with session
            Thread.Sleep(200);
            IdentificationCommand cmd = new IdentificationCommand(sessionToken);
            TcpSendData(JsonUtility.ToJson(cmd));
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
        return SendRequest(URL + uri, "POST", body);
            
    }

    private string SendGetRequest(string uri)
    {
        return SendRequest(URL + uri, "GET");
    }

    string SendRequest(string uri, string method, string body)
    {
        // We are blocking main thread waiting for response from the server
        // that way everything seems easy, no more waiting and blocking
        // @TODO: blocking main thread makes it impossible to implement 
        // progress bars, loaders etc.

        // @TODO: DRY
        Debug.Log("[" + method + "] " + uri);
        serverState = ServerState.TX;
        byte[] rawBody = System.Text.Encoding.ASCII.GetBytes(body);
        DownloadHandlerBuffer downloadHandler = new DownloadHandlerBuffer();

        UploadHandlerRaw uploadHandler = new UploadHandlerRaw(rawBody);
        uploadHandler.contentType = "application/json";
        UnityWebRequest request = new UnityWebRequest(uri, method, downloadHandler, uploadHandler);
        if (sessionToken != null)
            request.SetRequestHeader("session-token", sessionToken);
        request.SendWebRequest();
        WaitForSeconds w;
        while (!request.isDone)
            w = new WaitForSeconds(0.1f);
        Debug.Log("TX: " + request.uploadedBytes + " | RX" + request.downloadedBytes);
        lastTx = request.uploadedBytes;
        lastRx = request.downloadedBytes;
        totalTx += request.uploadedBytes;
        totalRx += request.downloadedBytes;
        lastResponseCode = request.responseCode;
        if (!request.isDone)
            Debug.LogError("NOT DONE");
        if (request.isHttpError)
        {
            lastError = request.error;
            OnError(request.error);
        }
        serverState = ServerState.RX;
        OnDataReceived(BytesToString(request.downloadHandler.data));
        serverState = ServerState.READY;
        return BytesToString(request.downloadHandler.data);
    }

    string SendRequest(string uri, string method)
    {
        // @TODO: DRY
        serverState = ServerState.TX;
        Debug.Log("[" + method + "] " + uri);
        UnityWebRequest request = UnityWebRequest.Get(uri);
        request.SendWebRequest();
        WaitForSeconds w;
        while (!request.isDone)
            w = new WaitForSeconds(0.1f);

        serverState = ServerState.RX;
        Debug.Log("TX: " + request.uploadedBytes + " | RX" + request.downloadedBytes);
        lastTx = request.uploadedBytes;
        lastRx = request.downloadedBytes;
        totalTx += request.uploadedBytes;
        totalRx += request.downloadedBytes;
        lastResponseCode = request.responseCode;
        if (request.isHttpError)
        {
            lastError = request.error;
            OnError(request.error);
        }

        OnDataReceived(BytesToString(request.downloadHandler.data));
        serverState = ServerState.READY;
        return BytesToString(request.downloadHandler.data);
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
                break;

            case "ack":
                Debug.Log("ACK");
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
        Byte[] buffer = new byte[1024];
        tcpConnected = true;

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
