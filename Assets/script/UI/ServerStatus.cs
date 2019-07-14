using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerStatus : MonoBehaviour
{

    private ApiClient apiClient = ApiClient.getInstance();
    public Text loggedInStatusText;
    public Text tcpStatusText;
    public Text transferText;
    public Text serverLoadText;

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
        {
            loggedInStatusText.text = "Not logged in.";
            return;
        }

        if (apiClient.IsTcpClientConnected())
            tcpStatusText.text = "TCP connected. " + apiClient.SecondsSinceLastPing().ToString();
        else
            tcpStatusText.text = "TCP disconnected.";

        if (apiClient.SecondsSinceLastPing() > 20)
            tcpStatusText.color = new Color(255, 0, 80);
        else
            tcpStatusText.color = new Color(255, 255, 255);

        transferText.text = "RX: " + apiClient.GetTotalRx() + " TX: " + apiClient.GetTotalTx();

        serverLoadText.text = "Clients online: " + apiClient.getStats().clientCount +
            " Battles in progress: " + apiClient.getStats().battleCount;
    }
}
