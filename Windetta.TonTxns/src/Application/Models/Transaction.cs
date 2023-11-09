namespace Windetta.TonTxns.Application.Models;

public struct Transaction
{
    public ulong Lt { get; set; }
    public string BodyHash { get; set; }
    public long Nanotons { get; set; }
    public string Comment { get; set; }
}
