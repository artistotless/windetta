namespace Windetta.Main.MatchHub;

internal class TournamentMatchHub : MatchHub
{
    public Guid OrganizerId { get; init; }
    public string? Description { get; init; }
    public string? Site { get; init; }

    public TournamentMatchHub(TournamentMatchHubOptions options) : base(options)
    {
        OrganizerId = options.OrganizerId;
        Description = options.Description;
        Site = options.Site;
    }
}
