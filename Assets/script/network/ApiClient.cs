using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ApiClient : MonoBehaviour
{

    private ServerState serverState = ServerState.READY;

    private string sessionToken;

    private string lastResponse;

    private ArrayList battles = new ArrayList();

    private string URL = "http://localhost:8080";

    private void Start()
    {
        EnterServer("Raff");
        ListBattles();
    }


    public void EnterServer(string username)
    {
        EnterServerRequest request = new EnterServerRequest(username);
        SendPostRequest("/user/enter", JsonUtility.ToJson(request));
    }

    public void ListBattles()
    {
        SendGetRequest("/user/battle-list");
    }


    private void SendPostRequest(string uri, string body)
    {
        StartCoroutine(SendRequest(URL + uri, "POST", body));
    }

    private void SendGetRequest(string uri)
    {
        StartCoroutine(SendRequest(URL + uri, "GET"));
    }

    IEnumerator SendRequest(string uri, string method, string body)
    {
        serverState = ServerState.TX;
        byte[] rawBody = System.Text.Encoding.ASCII.GetBytes(body);
        DownloadHandlerBuffer downloadHandler = new DownloadHandlerBuffer();

        UploadHandlerRaw uploadHandler = new UploadHandlerRaw(rawBody);
        uploadHandler.contentType = "application/json";
        UnityWebRequest request = new UnityWebRequest(uri, method, downloadHandler, uploadHandler);

        serverState = ServerState.TX;
        yield return request.SendWebRequest();
        OnDataReceived(BytesToString(request.downloadHandler.data));
    }

    IEnumerator SendRequest(string uri, string method)
    {
        serverState = ServerState.TX;

        Debug.Log("[" + method + "] " + uri);
        UnityWebRequest request = UnityWebRequest.Get(uri);
        yield return request.SendWebRequest();

        serverState = ServerState.READY;
        Debug.Log("TX: " + request.uploadedBytes + " | RX" + request.downloadedBytes);
        if (request.isHttpError)
            OnError(request.error);
        Debug.Log("Response code: " + request.responseCode);
        OnDataReceived(BytesToString(request.downloadHandler.data));
    }

    private string BytesToString(byte[] bytes)
    {
        return System.Text.Encoding.ASCII.GetString(bytes);
    }

    private void OnDataReceived(string data) {
        Debug.Log("Data received: " + data);
        serverState = ServerState.READY;
        lastResponse = data;
        GenericResponse genericResponse = JsonUtility.FromJson<GenericResponse>(data);
        if (genericResponse.sessionToken != null && sessionToken == null)
        {
            Debug.Log("received session token");
            sessionToken = genericResponse.sessionToken;
            return;
        }

        Debug.LogError("Unsupported response from the server. " + data);
    }

    private void OnError(string error)
    {
        Debug.LogError(error);
    }
}
