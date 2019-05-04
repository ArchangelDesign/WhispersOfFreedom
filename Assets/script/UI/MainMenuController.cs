using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{

    public InputField usernameInput;
    public GameObject battleListView;
    public Button connectButton;
    private ApiClient apiClient = ApiClient.getInstance();



    // Start is called before the first frame update
    void Start()
    {
        connectButton.onClick.AddListener(OnConnectClicked);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnConnectClicked()
    {
        if (apiClient.IsLoggedIn())
        {
            apiClient.LeaveServer();
            connectButton.GetComponentInChildren<Text>().text = "Connect";
            return;
        }
        string username = usernameInput.text.Trim();
        if (username == null)
            return;

        if (username.Length < 4)
            return;

        apiClient.EnterServer(username);
        connectButton.GetComponentInChildren<Text>().text = "Disconnect";

    }
}
