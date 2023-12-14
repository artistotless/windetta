using Windetta.Common.Types;

namespace Windetta.Main.Core.Matches;

public interface IMatchesHistory
{
    Task<Match> Get(DateRange dateRange, Guid? userId = null, Guid? gameId = null);
}