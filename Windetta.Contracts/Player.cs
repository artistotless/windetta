namespace Windetta.Contracts;

public record Player
{
    public Guid Id { get; init; }
    public string DisplayName { get; init; }
    public int TeamIndex { get; set; }

    public Player()
    {

    }

    public Player(Guid id, string displayName, int teamIndex)
    {
        Id = id;
        DisplayName = displayName;
        TeamIndex = teamIndex;
    }
}