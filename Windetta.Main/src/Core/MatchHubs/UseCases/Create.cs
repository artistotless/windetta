using Windetta.Main.Core.Exceptions;
using Windetta.Main.MatchHubs;
using Windetta.Main.Rooms;
using Windetta.Main.Services;

namespace Windetta.Main.Core.MatchHubs.UseCases;

public class Create : ICreateMatchHubUseCase
{
    private readonly IMatchHubs _hubs;
    private readonly IWalletService _walletService;
    private readonly IGetMatchHubIdByUserIdUseCase _getHubIdByUserIdUseCase;

    public Create(
        IMatchHubs hubs,
        IWalletService walletService,
        IGetMatchHubIdByUserIdUseCase getHubIdByUserIdUseCase)
    {
        _getHubIdByUserIdUseCase = getHubIdByUserIdUseCase;
        _hubs = hubs;
        _walletService = walletService;
    }

    public async Task<IMatchHub> ExecuteAsync(MatchHubOptions options)
    {
        if (!await _walletService.IsEqualOrGreater(
            options.InitiatorId, options.Bet.CurrencyId, options.Bet.Amount))
        {
            throw WalletException.FundsNotEnough;
        }

        if (options is TournamentMatchHubOptions o)
            return new TournamentMatchHub(o);

        var hubId = await _getHubIdByUserIdUseCase.ExecuteAsync(options.InitiatorId); ;

        if (hubId.HasValue)
            throw MatchHubException.AlreadyMemberOfHub;

        IMatchHub hub = new MatchHub(options);

        var initiator = new RoomMember(options.InitiatorId);

        hub.Add(initiator, hub.Rooms.First().Id);

        await _hubs.AddAsync(hub);

        return hub;
    }
}