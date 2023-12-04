using Windetta.Common.Types;
using Windetta.Main.Core.Exceptions;
using Windetta.Main.Core.MatchHubs;
using Windetta.Main.Core.MatchHubs.Dtos;
using Windetta.Main.Core.MatchHubs.UseCases;
using Windetta.Main.Games;
using Windetta.Main.MatchHubs.Filters;
using Windetta.Main.MatchHubs.Strategies;

namespace Windetta.Main.MatchHubs;

public class MatchHubsInteractor : IScopedService
{
    private readonly IGames _games;
    private readonly IMatchHubUsersAssociations _matchHubsUsersSets;
    private readonly IMatchHubUseCasesFactory _useCasesFactory;
    private readonly IMatchHubPluginsFactory _pluginsFactory;

    public MatchHubsInteractor(
        IGames games,
        IMatchHubPluginsFactory pluginsFactory,
        IMatchHubUseCasesFactory useCaseFactory,
        IMatchHubUsersAssociations? matchHubsUsersSets)
    {
        _matchHubsUsersSets = matchHubsUsersSets ??
            new InMemoryMatchHubUsersAssociations();

        _useCasesFactory = useCaseFactory;
        _pluginsFactory = pluginsFactory;
        _games = games;
    }

    public Task<Guid?> GetHubIdByUserId(Guid userId)
        => _useCasesFactory.Get<IGetMatchHubIdByUserIdUseCase>().ExecuteAsync(userId);

    public Task<IEnumerable<MatchHubDto>> GetAllAsync()
        => _useCasesFactory.Get<IGetAllMatchHubsUseCase>().ExecuteAsync();

    public async Task DeleteAsync(Guid hubId)
    {
        var roomsCache = (await _useCasesFactory.Get<IGetMatchHubUseCase>()
            .ExecuteAsync(hubId)).Rooms;

        await _useCasesFactory.Get<IDeleteMatchHubUseCase>().ExecuteAsync(hubId);

        _matchHubsUsersSets.Unset(roomsCache.SelectMany(r => r.Members, (r, m) => m.Id));
    }

    public async Task<IMatchHub> CreateAsync(CreateMatchHubRequest request)
    {
        var options = await BuildMatchHubOptions(request);

        var hub = await _useCasesFactory.Get<ICreateMatchHubUseCase>()
             .ExecuteAsync(options);

        _matchHubsUsersSets.Set(hub.Id, request.InitiatorId);

        return hub;
    }

    public async Task JoinMember(Guid userId, Guid hubId, Guid roomId)
    {
        await _useCasesFactory.Get<IJoinMemberMatchHubUseCase>()
            .ExecuteAsync(userId, hubId, roomId);

        _matchHubsUsersSets.Set(hubId, userId);
    }

    public async Task LeaveMember(Guid userId, Guid hubId)
    {
        await _useCasesFactory.Get<ILeaveMemberMatchHubUseCase>()
             .ExecuteAsync(userId, hubId);

        _matchHubsUsersSets.Unset(userId);
    }

    private async Task<MatchHubOptions> BuildMatchHubOptions(CreateMatchHubRequest request)
    {
        (GameConfiguration cfg, IEnumerable<SupportedCurrency> sc) configurations;

        configurations = await _games.GetConfigurationsAsync(request.GameId);

        ValidateBet(configurations.sc, request.Bet);

        MatchHubOptions options;

        if (request is CreateTournamentMatchHubRequest r)
        {
            options = new TournamentMatchHubOptions()
            {
                Private = request.Private,
                GameId = request.GameId,
                GameConfiguration = configurations.cfg,
                Bet = request.Bet,
                InitiatorId = request.InitiatorId,

                AutoDisposeStrategy = request.AutoDisposeStrategy is null ? null :
                _pluginsFactory.Get(request.AutoDisposeStrategy) as AutoDisposeStrategy,

                AutoReadyStrategy = request.AutoReadyStrategy is null ? null :
                _pluginsFactory.Get(request.AutoReadyStrategy) as AutoReadyStrategy,

                JoinFilters = request.JoinFilters is null ? null :
                request.JoinFilters.Select(f => (_pluginsFactory.Get(f) as IJoinFilter)!),

                Site = r.Site,
                Description = r.Description,
                OrganizerId = r.InitiatorId
            };
        }
        else
        {
            options = new MatchHubOptions()
            {
                Private = request.Private,
                GameId = request.GameId,
                GameConfiguration = configurations.cfg,
                Bet = request.Bet,
                InitiatorId = request.InitiatorId,

                AutoDisposeStrategy = request.AutoDisposeStrategy is null ? null :
                _pluginsFactory.GetOrDefaultImplementation<AutoDisposeStrategy>(request.AutoDisposeStrategy),

                AutoReadyStrategy = request.AutoReadyStrategy is null ? null :
                _pluginsFactory.GetOrDefaultImplementation<AutoReadyStrategy>(request.AutoReadyStrategy),

                JoinFilters = request.JoinFilters is null ? null :
                request.JoinFilters.Select(f => _pluginsFactory.GetOrDefaultImplementation<IJoinFilter>(f)),
            };
        }

        return options;
    }

    private static void ValidateBet(IEnumerable<SupportedCurrency> sc, Bet bet)
    {
        if (!sc.Any(s =>
            s.CurrencyId == bet.CurrencyId
            && bet.Amount <= s.MaxBet
            && bet.Amount >= s.MinBet))
        {
            throw MatchHubException.BetValidationFail;
        }
    }
}