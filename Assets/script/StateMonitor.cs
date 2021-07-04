using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using WofEngine;

public class StateMonitor : MonoBehaviour
{
    public readonly ConcurrentQueue<WOF_STATE> StateChangeEvents = new ConcurrentQueue<WOF_STATE>();

    private GameObject LoadingIndicator;

    public WOF_STATE LastState { get; private set; }
    void Start()
    {
        LoadingIndicator = GameObject.Find("LoadingIndicator");
        WofGameObject.Game.OnStateChanged += StateIsChanging;
        HideLoadingIndicator();
    }

    void Update()
    {
        if (StateChangeEvents.Count == 0)
            return;

        if (!StateChangeEvents.TryDequeue(out WOF_STATE newState))
            return;

        UpdateSceneForNewState(newState);
    }

    private void UpdateSceneForNewState(WOF_STATE newState)
    {
        Debug.Log("State update for " + newState);
        switch (newState)
        {
            case WOF_STATE.LOGINIG_IN:
                ShowLoadingIndicator();
                break;
            default:
                HideLoadingIndicator();
                break;
        }
        LastState = newState;
    }

    private void HideLoadingIndicator()
    {
        LoadingIndicator.SetActive(false);
    }

    private void ShowLoadingIndicator()
    {
        LoadingIndicator.SetActive(true);
    }

    private void StateIsChanging(WOF_STATE currentState)
    {
        StateChangeEvents.Enqueue(currentState);
    }
}
