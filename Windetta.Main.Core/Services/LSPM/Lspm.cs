namespace Windetta.Main.Core.Services.LSPM;

public class Lspm
{
    public Guid Id { get; set; }
    public IEnumerable<Guid> SupportedGames { get; set; }
    public int Load { get; set; }
    public Uri Endpoint { get; set; }
}