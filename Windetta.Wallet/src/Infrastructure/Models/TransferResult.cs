namespace Windetta.Wallet.Infrastructure.Models;

public class TransferResult
{
    public long Amount { get; set; }
    public string Destination { get; set; }
    public long TotalFee { get; set; }
}
