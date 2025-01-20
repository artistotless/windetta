namespace LSPM.Core.Models;

public class EndpointResolvingOptions
{
    public int HttpPortStart { get; set; }
    public int ProtocolPortStart { get; set; }
    public int MaxExposePorts { get; set; }
    public string LocalIp { get; set; }
    public string ExternalIp { get; set; }
    public string FetchIpApi { get; set; }
    public int FetchIpTimeout { get; set; }
}