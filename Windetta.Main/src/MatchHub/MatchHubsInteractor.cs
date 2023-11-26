using Windetta.Main.Rooms;

namespace Windetta.Main.MatchHub;

public class MatchHubsInteractor
{
    private readonly IMatchHubs _hubs;

    public MatchHubsInteractor(IMatchHubs hubs)
    {
        _hubs = hubs;
    }

    public async Task<IMatchHub> Create(MatchHubOptions options)
    {
        IMatchHub hub = new MatchHub(options);

        await _hubs.AddAsync(hub);

        return hub;
    }

    public async Task Delete(Guid hubId)
    {
        var hub = await _hubs.GetAsync(hubId);

        await _hubs.RemoveAsync(hub);

        hub.Dispose();
    }

    public async Task JoinMember(Guid userId, Guid hubId, Guid roomId)
    {
        var hub = await _hubs.GetAsync(hubId);

        JoinMember(userId, hub, roomId);
    }

    public void JoinMember(Guid userId, IMatchHub hub, Guid roomId)
    {
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
}