namespace Windetta.TonTxns.Application.Models;

public struct Transaction
{
    public Guid Id { get; set; }
    public ulong Amount { get; set; }
    public string Message { get; set; }
}
