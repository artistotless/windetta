namespace Windetta.Contracts;

public class ConnectionToServerDetails
{
    public Uri Endpoint { get; set; }
    public Dictionary<Guid, string> Tickets { get; set; }
}