using Windetta.Contracts;
using Windetta.Main.Core.Exceptions;
using Windetta.Main.Core.Lobbies.Plugins;
using Windetta.Main.Core.Rooms;
using Windetta.Main.Core.Services.Wallet;

namespace Windetta.Main.Core.Lobbies.UseCases;

public class Create : ICreateLobbyUseCase
{
    private readonly ILobbies _lobbies;
    private readonly IWalletService _walletService;

    public Create(ILobbies lobbies, IWalletService walletService)
    {
        _lobbies = lobbies;
        _walletService = walletService;
    }

    public async Task<ILobby> ExecuteAsync(LobbyOptions options)
    {
        if (!await _walletService.IsEqualOrGreater(
            options.InitiatorId, new FundsInfo(options.Bet.CurrencyId, options.Bet.Amount)))
        {
            throw WalletException.FundsNotEnough;
        }

        ILobby lobby;

        //TODO: delete guid parameter
        lobby = new Lobby(options, Guid.Parse("195da05a-d3ee-4d8b-917c-a77cf7afa906"));

        var initiator = new RoomMember(options.InitiatorId);

        lobby.Add(initiator, roomIndex: 0);

        lobby.SetAutoReadyStrategy(options.AutoReadyStrategy ??
            new DefaultReadyStrategy());

        lobby.SetDisposeStrategy(options.AutoDisposeStrategy ??
            new DefaultDisposeStrategy());

        await _lobbies.AddAsync(lobby);

        return lobby;
    }
}