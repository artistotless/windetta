using Windetta.Common.Constants;
using Windetta.Common.Types;
using Windetta.Main.Core.MatchHub;
using Windetta.Main.Core.MatchHub.Dtos;
using Windetta.Main.MatchHub.Filters;
using Windetta.Main.Rooms;

namespace Windetta.Main.MatchHub;

public class MatchHubsInteractor : IScopedService
{
    private readonly IMatchHubs _hubs;
    private readonly IMatchHubUsersAssociations _matchHubsUsersSets;
    private readonly IEnumerable<IJoinFilter>? _filters;

    public MatchHubsInteractor(IMatchHubs hubs,
        IMatchHubUsersAssociations? matchHubsUsersSets = null,
        IEnumerable<IJoinFilter>? filters = null)
    {
        _hubs = hubs;
        _filters = filters;
        _matchHubsUsersSets = matchHubsUsersSets?? new InMemoryMatchHubUsersAssociations();
    }

    public async Task<Guid?> GetHubIdByUserId(Guid userId)
        => _matchHubsUsersSets.GetHubId(userId);

    public async Task<IEnumerable<MatchHubDto>> GetAllAsync()
    {
        return await _hubs.GetAllAsync();
    }

    public async Task<IMatchHub> CreateAsync(MatchHubOptions options, Guid initiatorId)
    {
        var hubId = await GetHubIdByUserId(initiatorId);

        if (hubId.HasValue)
            throw new WindettaException(Errors.Main.CannotCreateMoreThanOneHub);

        IMatchHub hub = new MatchHub(options);

        var initiator = new RoomMember(initiatorId);

        hub.Add(initiator, hub.Rooms.First().Id);

        await _hubs.AddAsync(hub);

        return hub;
    }

    public async Task<IMatchHub> CreateAsync(TournamentMatchHubOptions options)
    {
        IMatchHub hub = new TournamentMatchHub(options);

        await _hubs.AddAsync(hub);

        return hub;
    }

    public async Task DeleteAsync(Guid hubId)
    {
        var hub = await _hubs.GetAsync(hubId);

        await _hubs.RemoveAsync(hub.Id);

        hub.Dispose();
    }

    public async Task JoinMember(Guid userId, Guid hubId)
    {
        var hub = await _hubs.GetAsync(hubId);
        var roomId = hub.Rooms.First().Id;

        await JoinMember(userId, hub, roomId);
    }

    public async Task JoinMember(Guid userId, Guid hubId, Guid roomId)
    {
        var hub = await _hubs.GetAsync(hubId);

        await JoinMember(userId, hub, roomId);
    }

    public async Task JoinMember(Guid userId, IMatchHub hub)
    {
        var roomId = hub.Rooms.First().Id;

        await JoinMember(userId, hub, roomId);
    }

    public async Task JoinMember(Guid userId, IMatchHub hub, Guid roomId)
    {
        if (_filters is not null && hub.JoinFilters is not null)
        {
            await ExecuteJoinFilters(userId, hub.JoinFilters);
        }

        var member = new RoomMember(userId);

        hub.Add(member, roomId);

        _matchHubsUsersSets.Set(hub.Id, userId);
    }

    public async Task LeaveMember(Guid userId, Guid hubId)
    {
        var hub = await _hubs.GetAsync(hubId);

        await LeaveMember(userId, hub);
    }

    public async Task LeaveMember(Guid userId, IMatchHub hub)
    {
        hub.Remove(userId);

        _matchHubsUsersSets.Remove(userId);
    }

    private async Task ExecuteJoinFilters(Guid userId, IReadOnlyCollection<string> joinFilters)
    {
        var cancelTokenSource = new CancellationTokenSource();
        var token = cancelTokenSource.Token;

        var filters = _filters!.Where(x => joinFilters.Contains(x.Name));
        var filterTasksQuery = filters.Select(x => x.ValidateAsync(userId, token));

        List<Task<(bool, string?)>> filterTasks = filterTasksQuery.ToList();

        bool allowToJoin = true;
        string errorMessage = string.Empty;

        while (filterTasks.Any())
        {
            var finishedTask = await Task.WhenAny(filterTasks);
            filterTasks.Remove(finishedTask);

            (allowToJoin, errorMessage) = await finishedTask;

            if (!allowToJoin)
            {
                // cancel other filter tasks
                cancelTokenSource.Cancel();
                break;
            }
        }

        if (!allowToJoin)
        {
            throw new WindettaException(
                Errors.Main.JoinFilterValidationFail, errorMessage);
        }
    }
}