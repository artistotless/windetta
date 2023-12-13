namespace Windetta.Main.Core.Domain.MatchHubs.UseCases;

public class GetHubIdByUserId : IGetMatchHubIdByUserIdUseCase
{
    private readonly IMatchHubUsersAssociations _matchHubsUsersSets;

    public GetHubIdByUserId(IMatchHubUsersAssociations matchHubsUsersSets)
    {
        _matchHubsUsersSets = matchHubsUsersSets;
    }

    public Task<Guid?> ExecuteAsync(Guid userId)
    {
        return Task.FromResult(_matchHubsUsersSets.GetHubId(userId));
    }
}