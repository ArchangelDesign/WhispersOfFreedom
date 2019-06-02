using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class WofUdpClient
{
    private static WofUdpClient instance;
    private UdpClient client;
    private Thread receiveThread;
    private string lastRawPacket;
    private bool serverAlive = true;
    private bool connected = false;
    private IPEndPoint endpoint;

    public static WofUdpClient GetInstance()
    {
        if (instance == null)
            instance = new WofUdpClient();

        return instance;
    }

    public WofUdpClient()
    {

    }

    public bool IsConnected()
    {
        return connected;
    }

    public void Kill()
    {
        serverAlive = false;
        connected = false;
        Debug.Log("UDP client killed.");
    }

    public void StartClient()
    {
        if (!ApiClient.getInstance().IsLoggedIn())
        {
            Debug.LogError("Cannot start UDP connection without client ID. REST server needs to be connected first.");
            return;
        }
        Debug.Log("Starting UDP client");
        endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8082);
        client = new UdpClient("127.0.0.1", 8082);
        client.Connect(endpoint);
        connected = true;
        serverAlive = true;
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
        // TODO: delay?
        Send(new IdentificationCommand(ApiClient.getInstance().GetClientId()));
    }

    public void Send(WofCommand command)
    {
        String rawJson = command.ToJson();
        byte[] dataToSend = Encoding.Encode(rawJson);
        client.Send(dataToSend, rawJson.Length);
    }

    private void ReceiveData()
    {
        while (serverAlive)
        {
            try
            {

                byte[] dataReceived = client.Receive(ref endpoint);
                lastRawPacket = System.Text.Encoding.ASCII.GetString(dataReceived);
                Debug.Log("UDP data received: " + lastRawPacket);
            } catch (Exception e)
            {
                connected = false;
                serverAlive = false;
                Debug.LogError("UDP receive error: " + e.StackTrace);
            }
        }
        Debug.Log("UDP client went offline.");
    }

    public void OnDestroy()
    {
        Debug.Log("Stoping UDP client...");
        connected = false;
        serverAlive = false;
        client.Close();
        receiveThread.Abort();
    }

}
