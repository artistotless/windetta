using Windetta.Contracts;
using Windetta.Main.Core.Exceptions;
using Windetta.Main.Core.MatchHubs.Plugins;
using Windetta.Main.Core.Rooms;
using Windetta.Main.Core.Services.Wallet;

namespace Windetta.Main.Core.MatchHubs.UseCases;

public class Create : ICreateMatchHubUseCase
{
    private readonly IMatchHubs _hubs;
    private readonly IWalletService _walletService;

    public Create(IMatchHubs hubs, IWalletService walletService)
    {
        _hubs = hubs;
        _walletService = walletService;
    }

    public async Task<IMatchHub> ExecuteAsync(MatchHubOptions options)
    {
        if (!await _walletService.IsEqualOrGreater(
            options.InitiatorId, new FundsInfo(options.Bet.CurrencyId, options.Bet.Amount)))
        {
            throw WalletException.FundsNotEnough;
        }

        IMatchHub hub;

        hub = new MatchHub(options);

        var initiator = new RoomMember(options.InitiatorId);

        hub.Add(initiator, hub.Rooms.First().Id);

        hub.SetAutoReadyStrategy(options.AutoReadyStrategy ??
            new DefaultReadyStrategy());

        hub.SetDisposeStrategy(options.AutoDisposeStrategy ??
            new DefaultDisposeStrategy());

        await _hubs.AddAsync(hub);

        return hub;
    }
}