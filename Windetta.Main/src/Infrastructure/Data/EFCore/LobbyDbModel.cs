using Windetta.Contracts;
using Windetta.Main.Core.Lobbies;
using Windetta.Main.Core.Lobbies.Dtos;

namespace Windetta.Main.Infrastructure.Data.EFCore;

public record BetDbModel(int CurrencyId, ulong Amount);
public class LobbyDbModel
{
    private string _instanceId;

    public Guid Id { get; set; }
    public Guid GameId { get; set; }
    public LobbyState State { get; set; }

    public DateTimeOffset Created { get; set; }
    public DateTimeOffset Updated { get; set; }
    public FundsInfo Bet { get; set; }
    public IEnumerable<string>? JoinFilters { get; set; }
    public IEnumerable<RoomDbModel> Rooms { get; set; }

    public static LobbyDbModel MapFrom(ILobby lobby)
    {
        return new LobbyDbModel()
        {
            Bet = lobby.Bet,
            GameId = lobby.GameId,
            Created = lobby.CreatedAt,
            Updated = lobby.UpdatedAt,
            State = lobby.State,
            Id = lobby.Id,
            JoinFilters = lobby.JoinFilters,
            Rooms = lobby.Rooms.Select(RoomDbModel.MapFrom),
        };
    }

    public static LobbyDto MapTo(LobbyDbModel dbModel)
    {
        return new LobbyDto()
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
