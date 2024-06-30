namespace Windetta.Contracts.Responses;

public sealed class GameServerResponse : ServiceResponse
{
    public Uri GameServerEndpoint { get; set; }
    public Guid GameServerId { get; set; }
}