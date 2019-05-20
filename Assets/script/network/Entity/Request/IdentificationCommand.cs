
using System;
using UnityEngine;

[Serializable]
public class IdentificationCommand : WofCommand
{
    public IdentificationCommand(string clientId)
    {
        this.clientId = clientId;
        this.command = "identification";
    }
}

