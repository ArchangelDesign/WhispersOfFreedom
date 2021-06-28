using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WofEngine;

public class LoginController : MonoBehaviour
{
    public static WofGameObject WofGo;

    void Start()
    {
        if (WofGo == null)
            WofGo = new WofGameObject();

    }


    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        switch (WofGo.State)
        {
            case WOF_STATE.LOGINIG_IN:
                // display progress dialog
                break;
            case WOF_STATE.LOGIN_FAILED:
            case WOF_STATE.LOGIN_SCREEN:
                // remove progress dialog
                break;
        }
    }

    public void Login()
    {
        InputField email = GameObject.Find("EmailInput").GetComponent<InputField>();
        InputField password = GameObject.Find("PasswordInput").GetComponent<InputField>();

        try
        {
            WofGo.EnterServer(email.text, password.text);
            Debug.Log("Login successfull");
        } catch (Exception e)
        {
            Debug.LogError(e.Message);
            Debug.LogError(e.StackTrace);
        }
    }
}
