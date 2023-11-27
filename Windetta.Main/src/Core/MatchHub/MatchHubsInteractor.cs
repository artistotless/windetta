using Windetta.Common.Constants;
using Windetta.Common.Types;
using Windetta.Main.MatchHub.Filters;
using Windetta.Main.Rooms;

namespace Windetta.Main.MatchHub;

public class MatchHubsInteractor : IScopedService
{
    private readonly IMatchHubs _hubs;
    private readonly IEnumerable<IJoinFilter>? _filters;

    public MatchHubsInteractor(IMatchHubs hubs, IEnumerable<IJoinFilter>? filters = null)
    {
        _hubs = hubs;
        _filters = filters;
    }

    public async Task<IEnumerable<IMatchHub>> GetAllAsync()
    {
        return await _hubs.GetAllAsync();
    }

    public async Task<IMatchHub> CreateAsync(MatchHubOptions options)
    {
        IMatchHub hub = new MatchHub(options);

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

        await _hubs.RemoveAsync(hub);

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
    }

    public async Task LeaveMember(Guid userId, Guid hubId)
    {
        var hub = await _hubs.GetAsync(hubId);

        LeaveMember(userId, hub);
    }

    public void LeaveMember(Guid userId, IMatchHub hub)
    {
        hub.Remove(userId);
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