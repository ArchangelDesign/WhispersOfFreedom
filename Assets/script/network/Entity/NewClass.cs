
public class Battle
{
    private string battleId { get; }

    private string name { get; }

    private int clients { get; }

    public Battle(string battleId, string name, int clients)
    {
        this.battleId = battleId;
        this.name = name;
        this.clients = clients;
    }
}
