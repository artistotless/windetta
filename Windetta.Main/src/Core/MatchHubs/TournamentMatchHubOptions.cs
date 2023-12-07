namespace Windetta.Main.Core.MatchHubs;

public sealed record TournamentMatchHubOptions : MatchHubOptions
{
    public Guid OrganizerId { get; set; }
    public string? Description { get; set; }
    public string? Site { get; set; }
}
