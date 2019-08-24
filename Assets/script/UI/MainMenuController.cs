using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{

    public InputField usernameInput;
    public InputField passwordInput;
    public GameObject battleListView;
    public Button connectButton;
    private ApiClient apiClient = ApiClient.getInstance();
    private GameController gameController;
    private Notification NotificationController;



    // Start is called before the first frame update
    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        NotificationController = FindObjectOfType<Notification>();
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
        string password = passwordInput.text.Trim();
        if (username == null || username.Length < 4) {
            NotificationController.Error("Invalid username");            
            return;
        }

        if (apiClient.EnterServer(username, password))
        {
            connectButton.GetComponentInChildren<Text>().text = "Disconnect";

        } else
        {
            NotificationController.Error(apiClient.GetLastError());
        }
        

    }
}
