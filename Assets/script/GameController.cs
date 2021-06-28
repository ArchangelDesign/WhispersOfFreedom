using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    private ApiClient apiClient = ApiClient.getInstance();
    private WofUdpClient udpClient = WofUdpClient.GetInstance();

    public GameObject MainMenuCanvas;

    void Start()
    {
    }
    
    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        apiClient.OnDestroy();
        udpClient.OnDestroy();
    }

    public void HideMainMenu()
    {
        MainMenuCanvas.SetActive(false);
    }

    public void ShowMainMenu()
    {
        MainMenuCanvas.SetActive(true);
    }

}
