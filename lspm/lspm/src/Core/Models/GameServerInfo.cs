namespace LSPM.Core.Models;

public class GameServerInfo
{
    public Guid InstanceId { get; set; }
    public Uri Endpoint { get; set; }
    public Uri IpcEndpoint { get; set; }
    public bool IsReady { get; set; }
}