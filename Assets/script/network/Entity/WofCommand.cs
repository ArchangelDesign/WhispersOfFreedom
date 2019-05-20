using UnityEngine;

public class WofCommand
{
    public string command;
    public string clientId;
    public string arg1;
    public string memo;


    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
}
