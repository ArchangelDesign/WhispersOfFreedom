using System;
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
    private GameController gameController;



    // Start is called before the first frame update
    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        if (gameController == null)
            throw new Exception("Game Controller not found in the scene.");
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
        if (username == null || username.Length < 4) {
            gameController.Notification("Invalid username");
            return;
        }

        if (username.Length < 4)
            return;

        apiClient.EnterServer(username);
        connectButton.GetComponentInChildren<Text>().text = "Disconnect";

    }
}
