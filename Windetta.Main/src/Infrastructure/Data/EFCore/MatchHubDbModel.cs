using Windetta.Main.Core.Domain.MatchHubs;
using Windetta.Main.Core.Domain.MatchHubs.Dtos;

namespace Windetta.Main.Infrastructure.Data.EFCore;

public class MatchHubDbModel
{
    private string _instanceId;

    public Guid Id { get; set; }
    public Guid GameId { get; set; }
    public MatchHubState State { get; set; }

    public DateTimeOffset Created { get; set; }
    public DateTimeOffset Updated { get; set; }
    public Bet Bet { get; set; }
    public IEnumerable<string>? JoinFilters { get; set; }
    public IEnumerable<RoomDbModel> Rooms { get; set; }

    // Tournament part
    public Guid? OrganizerId { get; set; }
    public string? Description { get; set; }
    public string? Site { get; set; }

    public static MatchHubDbModel MapFrom(IMatchHub matchHub)
    {
        TournamentMatchHub? tournamentMatchHub = matchHub as TournamentMatchHub;

        return new MatchHubDbModel()
        {
            Bet = matchHub.Bet,
            GameId = matchHub.GameId,
            Created = matchHub.CreatedAt,
            Updated = matchHub.UpdatedAt,
            State = matchHub.State,
            Id = matchHub.Id,
            JoinFilters = matchHub.JoinFilters,
            OrganizerId = tournamentMatchHub?.OrganizerId,
            Description = tournamentMatchHub?.Description,
            Site = tournamentMatchHub?.Site,
            Rooms = matchHub.Rooms.Select(RoomDbModel.MapFrom),
        };
    }

    public static MatchHubDto MapTo(MatchHubDbModel dbModel)
    {
        if (dbModel.OrganizerId != null)
        {
            return new TournamentMatchHubDto()
            {
                OrganizerId = dbModel.OrganizerId.Value,
                Description = dbModel.Description,
                State = dbModel.State,
                GameId = dbModel.GameId,
                Site = dbModel.Site,
                Bet = dbModel.Bet,
                Created = dbModel.Created,
                Updated = dbModel.Updated,
                Id = dbModel.Id,
                JoinFilters = dbModel.JoinFilters,
                Rooms = dbModel.Rooms.Select(RoomDbModel.MapTo),
            };
        }
        else
        {
            return new MatchHubDto()
            {
                Bet = dbModel.Bet,
                State = dbModel.State,
                GameId = dbModel.GameId,
                Created = dbModel.Created,
                Updated = dbModel.Updated,
                Id = dbModel.Id,
                JoinFilters = dbModel.JoinFilters,
                Rooms = dbModel.Rooms.Select(RoomDbModel.MapTo)
            };
        }
    }
}
