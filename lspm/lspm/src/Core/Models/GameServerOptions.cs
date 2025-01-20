namespace LSPM.Core.Models;

public class GameServerOptions
{
    public Guid GameId { get; set; }
    public string Path { get; set; }
    public string ExecutableFile { get; set; }
    public string Protocol { get; set; }
    public int MaxMatchesOnInstance { get; set; }
    public bool NeedProtocolPort { get; set; }
    public int MaxInstances { get; set; }
    public int MaxMatchDurationTimeout { get; set; }
    public string RunTool { get; set; } = string.Empty;
    public string EndpointFormat { get; set; }
}