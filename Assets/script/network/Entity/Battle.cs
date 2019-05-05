
using System;

[Serializable]
public class Battle
{
    public string battleId;

    public string name;

    public int clients;

    public Battle() { }

    public Battle(string battleId, string name, int clients)
    {
        this.battleId = battleId;
        this.name = name;
        this.clients = clients;
    }
}
