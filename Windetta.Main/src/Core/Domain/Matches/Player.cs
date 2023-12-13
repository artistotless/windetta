namespace Windetta.Main.Core.Domain.Matches;

public class Player
{
    Guid Id { get; init; }
    public string DisplayName { get; init; }
    public int TeamIndex { get; set; }
}