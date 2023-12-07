namespace Windetta.Main.Core.MatchHubs;

public class TournamentMatchHub : MatchHub
{
    public Guid OrganizerId { get; init; }
    public string? Description { get; init; }
    public string? Site { get; init; }

    internal TournamentMatchHub(TournamentMatchHubOptions options) : base(options)
    {
        OrganizerId = options.OrganizerId;
        Description = options.Description;
        Site = options.Site;
    }
}
