using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WofEngine;

public class LoginController : MonoBehaviour
{
    private WOF_STATE PreviousState;

    void Start()
    {
        WofGameObject.Game.StateChangedCallback += StateHasChanged;
    }


    void Update()
    {
        
    }

    private void FixedUpdate()
    {
    }

    public void Login()
    {
        InputField email = GameObject.Find("EmailInput").GetComponent<InputField>();
        InputField password = GameObject.Find("PasswordInput").GetComponent<InputField>();

        WofGameObject.Game.EnterServer(email.text, password.text);
        PreviousState = WofGameObject.Game.State;
    }

    private void StateHasChanged(WOF_STATE newState)
    {
        Debug.Log("new state: " + newState);
        switch (newState)
        {
            case WOF_STATE.LOGIN_FAILED:
                if (PreviousState == WOF_STATE.LOGINIG_IN)
                    LoginFailed();
                break;
            case WOF_STATE.LOBBY:
                if (PreviousState == WOF_STATE.LOGINIG_IN)
                    LoginSuccessfull();
                break;
            case WOF_STATE.LOGINIG_IN:
                Debug.Log("Logging in");
                break;
        }
    }

    private void LoginFailed()
    {
        Debug.LogError("Login failed");
    }

    private void LoginSuccessfull()
    {
        Debug.Log("Login successfull");
        try
        {
            WofGameObject.Game.InitializeTcpConnection();
        } catch (TcpConnectionError)
        {
            Debug.LogError("TCP connection error");
        }
    }
}
