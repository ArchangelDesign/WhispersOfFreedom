using System;
using UnityEngine;

public class EnterBattleRequest
{
    public string battleId;

    public EnterBattleRequest() { }

    public EnterBattleRequest(string battleId)
    {
        this.battleId = battleId;
    }

    internal string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
}