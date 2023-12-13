using Windetta.Common.Types;

namespace Windetta.Main.Core.Domain.Matches;

public interface IMatchesHistory
{
    Task<Match> Get(DateRange dateRange, Guid? userId = null, Guid? gameId = null);
}