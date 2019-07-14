
using System.Collections;

[System.Serializable]
public class Parameters
{
    public string clientCount;
    public string battleCount;
    public string inBattle;
    public string connected;
    public string currentTime;
}

[System.Serializable]
public class GenericResponse
{
    public string clientId;
    public string sessionToken;
    public string arg1;
    public string memo;
    // looks like JsonUtility does not support arrays nor hashtables
    // @TODO
    public Parameters parameters = new Parameters();
}
