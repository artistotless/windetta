namespace Windetta.MainTests.Mocks;

public class FakeMatchHub : Main.Core.MatchHubs.MatchHub
{

    public FakeMatchHub(MatchHubOptions options, Guid? id = null) : base(options, id)
    {
    }

    public override IEnumerable<IJoinFilter>? GetJoinFilters()
    {
        return null;
    }

    public new void Remove(Guid memberId)
    {

    }
}
