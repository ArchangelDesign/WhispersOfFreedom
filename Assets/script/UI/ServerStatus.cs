using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerStatus : MonoBehaviour
{

    private ApiClient apiClient = ApiClient.getInstance();
    public Text loggedInStatusText;
    public Text tcpStatusText;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (apiClient.IsLoggedIn())
            loggedInStatusText.text = "Logged in.";
        else
            loggedInStatusText.text = "Not logged in.";

        if (apiClient.IsTcpClientConnected())
            tcpStatusText.text = "TCP connected.";
        else
            tcpStatusText.text = "TCP disconnected.";

    }
}
