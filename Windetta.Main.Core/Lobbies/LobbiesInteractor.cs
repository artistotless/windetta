using Windetta.Common.Types;
using Windetta.Contracts;
using Windetta.Main.Core.Exceptions;
using Windetta.Main.Core.Games;
using Windetta.Main.Core.Lobbies.Dtos;
using Windetta.Main.Core.Lobbies.Plugins;
using Windetta.Main.Core.Lobbies.UseCases;

namespace Windetta.Main.Core.Lobbies;

public sealed class LobbiesInteractor : IScopedService
{
    private readonly IGames _gamesRepository;
    private readonly IUserLobbyMaps _lobbyUserMaps;
    private readonly ILobbyUseCasesFactory _useCasesFactory;
    private readonly ILobbyPluginsFactory _pluginsFactory;

    public LobbiesInteractor(
        IGames gamesRepository,
        ILobbyPluginsFactory pluginsFactory,
        ILobbyUseCasesFactory useCaseFactory,
        IUserLobbyMaps? lobbiesUsersSets = null)
    {
        _lobbyUserMaps = lobbiesUsersSets ??
            new InMemoryLobbyUserMaps();

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
        var rooms = (await _useCasesFactory.Get<IGetLobbyUseCase>()
            .ExecuteAsync(lobbyId)).Rooms;

        _lobbyUserMaps.Unset(rooms.SelectMany(r => r.Members, (r, m) => m.Id));

        await _useCasesFactory.Get<IDeleteLobbyUseCase>().ExecuteAsync(lobbyId);
    }

    public async Task<ILobby> CreateAsync(CreateLobbyDto request, Guid initiatorId)
    {
        if (_lobbyUserMaps.Get(initiatorId).HasValue)
            throw LobbyException.AlreadyMemberOfLobby;

        (GameConfiguration cfg, IEnumerable<SupportedCurrency> sc) configurations;
        configurations = await _gamesRepository.GetConfigurationsAsync(request.GameId);

        ValidateBet(configurations.sc, request.Bet);
        ValidateSlots(configurations.cfg, request);

        var options = await BuildLobbyOptions(request, initiatorId);

        var lobby = await _useCasesFactory.Get<ICreateLobbyUseCase>()
             .ExecuteAsync(options);

        _lobbyUserMaps.Set(new(initiatorId, lobby.Id, roomIndex: 0));

        return lobby;
    }

    public async Task JoinMemberAsync(Guid userId, Guid lobbyId, ushort roomIndex)
    {
        await _useCasesFactory.Get<IJoinMemberLobbyUseCase>()
            .ExecuteAsync(userId, lobbyId, roomIndex);

        _lobbyUserMaps.Set(new(userId, lobbyId, roomIndex));
    }

    public async Task LeaveMemberAsync(Guid userId, Guid lobbyId, ushort? roomIndex = null)
    {
        if (roomIndex is null)
        {
            var userLocation = _lobbyUserMaps.Get(userId);

            if (!userLocation.HasValue)
                throw LobbyException.MemberAreNotInRoom;

            roomIndex = userLocation.Value.RoomIndex;
        }

        await _useCasesFactory.Get<ILeaveMemberLobbyUseCase>()
             .ExecuteAsync(userId, lobbyId, roomIndex.Value);

        _lobbyUserMaps.Unset(userId);
    }

    private async Task<LobbyOptions> BuildLobbyOptions(CreateLobbyDto request, Guid InitiatorId)
    {
        var options = new LobbyOptions()
        {
            Private = request.Private,
            GameId = request.GameId,
            Properties = request.Properties,

            Bet = request.Bet,
            InitiatorId = InitiatorId,
            Teams = request.Teams,
            Slots = request.Slots,

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

    private static void ValidateBet(IEnumerable<SupportedCurrency> sc, FundsInfo bet)
    {
        if (!sc.Any(s =>
            s.CurrencyId == bet.CurrencyId
            && bet.Amount <= s.MaxBet
            && bet.Amount >= s.MinBet))
        {
            throw LobbyException.BetValidationFail;
        }
    }

    private static void ValidateSlots(GameConfiguration cfg, CreateLobbyDto createDto)
    {
        if (createDto.Teams > cfg.MaxTeams)
            throw LobbyException.TeamsAreGreaterMaximumAllowed;

        else if (createDto.Teams < cfg.MinTeams)
            throw LobbyException.TeamsAreLessMinimumRequired;

        else if (createDto.Slots > cfg.MaxPlayersInTeam)
            throw LobbyException.SlotsAreGreaterMaximumAllowed;

        else if (createDto.Slots < cfg.MinTeams)
            throw LobbyException.SlotsAreLessMinimumRequired;
    }
}