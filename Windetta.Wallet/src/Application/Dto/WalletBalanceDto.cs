namespace Windetta.Wallet.Application.Dto;

public class WalletBalanceDto
{
    public long Total { get; init; }
    public long Held { get; init; }

    public WalletBalanceDto(long total, long held)
    {
        Total = total;
        Held = held;
    }
}