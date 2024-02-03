using Windetta.Contracts;
using Windetta.Main.Core.Exceptions;
using Windetta.Main.Core.Lobbies.Plugins;
using Windetta.Main.Core.Rooms;
using Windetta.Main.Core.Services.Wallet;

namespace Windetta.Main.Core.Lobbies.UseCases;

public class JoinMember : IJoinMemberLobbyUseCase
{
    private readonly ILobbies _lobbies;
    private readonly IWalletService _walletService;

    public JoinMember(ILobbies lobbies, IWalletService walletService)
    {
        _lobbies = lobbies;
        _walletService = walletService;
    }

    public async Task ExecuteAsync(Guid userId, Guid lobbyId, ushort roomIndex)
    {
        var lobby = await _lobbies.GetAsync(lobbyId);

        if (lobby is null)
            throw LobbyException.NotFound;

        if (!await _walletService.IsEqualOrGreater(
            userId, new FundsInfo(lobby.Bet.CurrencyId, lobby.Bet.Amount)))
        {
            throw WalletException.FundsNotEnough;
        }

        var joinFilters = lobby.GetJoinFilters();

        if (joinFilters is not null)
            await joinFilters.ExecuteFiltersAsync(userId);

        var member = new RoomMember(userId);

        lobby.Add(member, roomIndex);
    }
}