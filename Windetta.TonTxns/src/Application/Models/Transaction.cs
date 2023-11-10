namespace Windetta.TonTxns.Application.Models;

public struct Transaction
{
    public ulong Id { get; set; }
    public ulong TimeStamp { get; set; }
    public string BodyHash { get; set; }
    public long Amount { get; set; }
    public string Message { get; set; }
}
