using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

public class ApiClient : MonoBehaviour
{

    // MAX wait time for response from REST server
    public const long TIMEOUT = 30;

    private ServerState serverState = ServerState.READY;

    private string URL = "http://localhost:8080";

    private string TcpServerUrl = "localhost";

    private int TcpServerPort = 8081;

    private string sessionToken;

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

    private void Start()
    {
        EnterServer("Raff");
        //ListBattles();
    }


    public void EnterServer(string username)
    {
        if (loggedIn)
            return;
        EnterServerRequest request = new EnterServerRequest(username);
        lastResponse = null;

        string response = SendPostRequest("/user/enter", JsonUtility.ToJson(request));
        GenericResponse genericResponse = JsonUtility.FromJson<GenericResponse>(response);
        if (genericResponse.sessionToken != null)
        {
            Debug.Log("received session token");
            sessionToken = genericResponse.sessionToken;
            loggedIn = true;
            InitiateTcpConnection();
        }
    }

    private void InitiateTcpConnection()
    {
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
        battles = JsonUtility.FromJson<Battle[]>(lastResponse);
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

    public void CreateBattle() {
        if (inBattle)
            return;


    }

    public void TerminateBattle() { }

    public Battle[] getBattles()
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
        request.SendWebRequest();
        WaitForSeconds w;
        while (!request.isDone)
            w = new WaitForSeconds(0.1f);
        Debug.Log("TX: " + request.uploadedBytes + " | RX" + request.downloadedBytes);
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
        return System.Text.Encoding.ASCII.GetString(bytes);
    }

    private void OnDataReceived(string data) {
        Debug.Log("Data received: " + data);
        lastResponse = data;
    }

    private void OnTcpDataReceived(string data)
    {
        Debug.Log("TCP data received: " + data);
        WofCommand cmd = JsonUtility.FromJson<WofCommand>(data);

        switch (cmd.command)
        {
            case "ping":
                break;

            case "ack":
                Debug.Log("ACK");
                break;
        }

        Debug.LogError("Unhandled command from server: " + cmd.command);
    }

    private void TcpDataHandler()
    {
        tcpClient = new TcpClient(TcpServerUrl, TcpServerPort);
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
                tcpLastResponse = System.Text.Encoding.ASCII.GetString(receivedBytes);
                OnTcpDataReceived(tcpLastResponse);
            }
        }
        Debug.LogError("Server disconnected");
    }

    private void TcpSendData(string data)
    {
        if (!tcpConnected)
            return;

        NetworkStream stream = tcpClient.GetStream();

        if (!stream.CanWrite)
        {
            Debug.LogError("Cannot write to TCP stream.");
            return;
        }

        byte[] rawData = System.Text.Encoding.ASCII.GetBytes(data);
        stream.Write(rawData, 0, rawData.Length);
    }

    private void OnError(string error)
    {
        Debug.LogError(error);
    }
}
