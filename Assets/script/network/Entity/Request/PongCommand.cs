using System;

[Serializable]
public class PongCommand : WofCommand
{
    public PongCommand(string clientId)
    {
        this.clientId = clientId;
        this.command = "pong";
    }
}