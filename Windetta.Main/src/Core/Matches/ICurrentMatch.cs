namespace Windetta.Main.Core.Matches;

public interface ICurrentMatch
{
    Task<Match> GetAsync(Guid userId);
}
