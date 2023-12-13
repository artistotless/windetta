namespace Windetta.Main.Core.Domain.MatchHubs;

public class TournamentMatchHub : MatchHub
{
    public Guid OrganizerId { get; init; }
    public string? Description { get; init; }
    public string? Site { get; init; }

    public TournamentMatchHub(TournamentMatchHubOptions options, Guid? id = null) : base(options, id)
    {
        OrganizerId = options.OrganizerId;
        Description = options.Description;
        Site = options.Site;
    }
}
