using Windetta.Contracts.Responses;

namespace LSPM.Models;

public sealed class GameServerResponse : ServiceResponse
{
    public Uri GameServerEndpoint { get; set; }
    public Guid GameServerId { get; set; }
}