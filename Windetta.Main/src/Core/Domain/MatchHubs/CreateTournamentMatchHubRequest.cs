namespace Windetta.Main.Core.Domain.MatchHubs;

public class CreateTournamentMatchHubRequest : CreateMatchHubRequest
{
    public string? Description { get; set; }
    public string? Site { get; set; }
}