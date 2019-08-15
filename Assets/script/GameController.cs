using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    private ApiClient apiClient = ApiClient.getInstance();
    private WofUdpClient udpClient = WofUdpClient.GetInstance();

    void Start()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        //apiClient.OnDestroy();
        //udpClient.OnDestroy();
    }


}
