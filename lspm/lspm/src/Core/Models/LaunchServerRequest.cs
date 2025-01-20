using LSPM.Models;

namespace LSPM.Core.Models;

public class LaunchServerRequest
{
    public Guid GameId { get; set; }
    public MatchInitializationData MatchInitData { get; set; }
}
