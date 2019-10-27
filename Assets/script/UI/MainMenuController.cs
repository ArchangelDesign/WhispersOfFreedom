using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

    public InputField usernameInput;
    public InputField passwordInput;
    public GameObject battleListView;
    public Button connectButton;
    public Button createLobbyButton;
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
        createLobbyButton.onClick.AddListener(OnCreateLobbyClicked);
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

        }
        else
        {
            NotificationController.Error(apiClient.GetLastError());
        }
    }

    public void OnCreateLobbyClicked()
    {
        if (!apiClient.IsLoggedIn())
        {
            NotificationController.Error("Not connected.");
            return;
        }
        gameController.HideMainMenu();
        SceneManager.LoadScene("BattleLobby", LoadSceneMode.Additive);
    }
}
