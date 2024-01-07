using Windetta.Common.Types;
using Windetta.Main.Core.Exceptions;
using Windetta.Main.Core.Games;
using Windetta.Main.Core.MatchHubs.Dtos;
using Windetta.Main.Core.MatchHubs.Plugins;
using Windetta.Main.Core.MatchHubs.UseCases;

namespace Windetta.Main.Core.MatchHubs;

public class MatchHubsInteractor : IScopedService
{
    private readonly IGames _gamesRepository;
    private readonly IMatchHubUsersAssociations _matchHubsUsersSets;
    private readonly IMatchHubUseCasesFactory _useCasesFactory;
    private readonly IMatchHubPluginsFactory _pluginsFactory;

    public MatchHubsInteractor(
        IGames gamesRepository,
        IMatchHubPluginsFactory pluginsFactory,
        IMatchHubUseCasesFactory useCaseFactory,
        IMatchHubUsersAssociations? matchHubsUsersSets = null)
    {
        _matchHubsUsersSets = matchHubsUsersSets ??
            new InMemoryMatchHubUsersAssociations();

        _useCasesFactory = useCaseFactory;
        _pluginsFactory = pluginsFactory;
        _gamesRepository = gamesRepository;
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
        if (_matchHubsUsersSets.GetHubId(request.InitiatorId).HasValue)
            throw MatchHubException.AlreadyMemberOfHub;

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

    // TODO: refactor it, too big size of method & duplicated code
    private async Task<MatchHubOptions> BuildMatchHubOptions(CreateMatchHubRequest request)
    {
        (GameConfiguration cfg, IEnumerable<SupportedCurrency> sc) configurations;

        configurations = await _gamesRepository.GetConfigurationsAsync(request.GameId);

        ValidateBet(configurations.sc, request.Bet);

        var options = new MatchHubOptions()
        {
            Private = request.Private,
            GameId = request.GameId,
            GameConfiguration = configurations.cfg,
            Bet = request.Bet,
            InitiatorId = request.InitiatorId,

            AutoDisposeStrategy = request.AutoDisposeStrategy is null ? null :
            _pluginsFactory.Get<IAutoDisposeStrategy>(
                request.AutoDisposeStrategy.Name,
                request.AutoDisposeStrategy.RequirementsValues),

            AutoReadyStrategy = request.AutoReadyStrategy is null ? null :
            _pluginsFactory.Get<IAutoReadyStrategy>(
                request.AutoReadyStrategy.Name,
                request.AutoReadyStrategy.RequirementsValues),

            JoinFilters = request.JoinFilters is null ? null :
            request.JoinFilters.Select(f => _pluginsFactory
            .Get<IJoinFilter>(f.Name, f.RequirementsValues)),
        };

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