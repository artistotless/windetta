using Windetta.Common.Types;
using Windetta.Main.Core.Exceptions;
using Windetta.Main.Core.Games;
using Windetta.Main.Core.Lobbies.Dtos;
using Windetta.Main.Core.Lobbies.Plugins;
using Windetta.Main.Core.Lobbies.UseCases;

namespace Windetta.Main.Core.Lobbies;

public sealed class LobbiesInteractor : IScopedService
{
    private readonly IGames _gamesRepository;
    private readonly ILobbyUsersAssociations _lobbiesUsersSets;
    private readonly ILobbyUseCasesFactory _useCasesFactory;
    private readonly ILobbyPluginsFactory _pluginsFactory;

    public LobbiesInteractor(
        IGames gamesRepository,
        ILobbyPluginsFactory pluginsFactory,
        ILobbyUseCasesFactory useCaseFactory,
        ILobbyUsersAssociations? lobbiesUsersSets = null)
    {
        _lobbiesUsersSets = lobbiesUsersSets ??
            new InMemoryLobbyUsersAssociations();

        _useCasesFactory = useCaseFactory;
        _pluginsFactory = pluginsFactory;
        _gamesRepository = gamesRepository;
    }

    public Task<Guid?> GetLobbyIdByUserIdAsync(Guid userId)
        => _useCasesFactory.Get<IGetLobbyIdByUserIdUseCase>().ExecuteAsync(userId);

    public Task<IEnumerable<LobbyDto>> GetAllAsync()
        => _useCasesFactory.Get<IGetAllLobbiesUseCase>().ExecuteAsync();

    public async Task DeleteAsync(Guid lobbyId)
    {
        var roomsCache = (await _useCasesFactory.Get<IGetLobbyUseCase>()
            .ExecuteAsync(lobbyId)).Rooms;

        await _useCasesFactory.Get<IDeleteLobbyUseCase>().ExecuteAsync(lobbyId);

        _lobbiesUsersSets.Unset(roomsCache.SelectMany(r => r.Members, (r, m) => m.Id));
    }

    public async Task<ILobby> CreateAsync(CreateLobbyDto request)
    {
        if (_lobbiesUsersSets.GetLobbyId(request.InitiatorId).HasValue)
            throw LobbyException.AlreadyMemberOfLobby;

        var options = await BuildLobbyOptions(request);

        var lobby = await _useCasesFactory.Get<ICreateLobbyUseCase>()
             .ExecuteAsync(options);

        _lobbiesUsersSets.Set(lobby.Id, request.InitiatorId);

        return lobby;
    }

    public async Task JoinMemberAsync(Guid userId, Guid lobbyId, ushort roomIndex)
    {
        await _useCasesFactory.Get<IJoinMemberLobbyUseCase>()
            .ExecuteAsync(userId, lobbyId, roomIndex);

        _lobbiesUsersSets.Set(lobbyId, userId);
    }

    public async Task LeaveMemberAsync(Guid userId, Guid lobbyId, ushort roomIndex)
    {
        await _useCasesFactory.Get<ILeaveMemberLobbyUseCase>()
             .ExecuteAsync(userId, lobbyId, roomIndex);

        _lobbiesUsersSets.Unset(userId);
    }

    private async Task<LobbyOptions> BuildLobbyOptions(CreateLobbyDto request)
    {
        (GameConfiguration cfg, IEnumerable<SupportedCurrency> sc) configurations;

        configurations = await _gamesRepository.GetConfigurationsAsync(request.GameId);

        ValidateBet(configurations.sc, request.Bet);

        var options = new LobbyOptions()
        {
            Private = request.Private,
            GameId = request.GameId,
            GameConfiguration = configurations.cfg,
            Properties = request.Properties,
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
            throw LobbyException.BetValidationFail;
        }
    }
}