namespace Windetta.Wallet.Application.Dto;

public class WalletInfoDto
{
    public WalletBalanceDto Balance { get; init; }

    public WalletInfoDto(WalletBalanceDto balance)
    {
        Balance = balance;
    }
}
