namespace Windetta.Main.Core.Domain.Matches;

public interface ICurrentMatch
{
    Task<Match> GetAsync(Guid userId);
}
