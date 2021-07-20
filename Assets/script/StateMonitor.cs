using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WofEngine;
using WofEngine.NetworkPacket;

public class StateMonitor : MonoBehaviour
{
    public readonly ConcurrentQueue<WOF_STATE> StateChangeEvents = new ConcurrentQueue<WOF_STATE>();

    private GameObject LoadingIndicator;
    private Text UdpLatencyText;

    public WOF_STATE LastState { get; private set; }
    void Start()
    {
        LoadingIndicator = GameObject.Find("LoadingIndicator");
        UdpLatencyText = GameObject.Find("UDPLatency").GetComponent<Text>();
        WofGameObject.Game.OnStateChanged += StateIsChanging;
        WofGameObject.Game.OnPacketReceived += PacketReceived;
        HideLoadingIndicator();
    }

    private void PacketReceived(GenericNetworkPacket packet)
    {
        long time = packet.GetTime();
        double currentTime = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        long currentMillis = DateTime.UtcNow.Millisecond;
        Debug.Log("packetTime: " + time + " current: " + currentTime + " current millis: " + currentMillis);
        Debug.Log("latency: " + packet.GetLatencyMilliseconds());
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
