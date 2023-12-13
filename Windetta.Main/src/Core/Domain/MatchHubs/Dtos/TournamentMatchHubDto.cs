namespace Windetta.Main.Core.Domain.MatchHubs.Dtos;

public class TournamentMatchHubDto : MatchHubDto
{
    public Guid OrganizerId { get; init; }
    public string? Description { get; init; }
    public string? Site { get; init; }

    public TournamentMatchHubDto() { }

    public TournamentMatchHubDto(IMatchHub mapFrom) : base(mapFrom)
    {
        if (mapFrom is TournamentMatchHub mapped)
        {
            OrganizerId = mapped.OrganizerId;
            Description = mapped.Description;
            Site = mapped.Site;
        }
        else
        {
            throw new InvalidCastException();
        }
    }
}
