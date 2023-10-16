using Windetta.Wallet.Domain;

namespace Windetta.Wallet.Application.Dto;

public class WalletInfoDto
{
    public WalletBalanceDto Balance { get; init; }
    public string Address { get; init; }

    public WalletInfoDto(WalletBalanceDto balance, string address)
    {
        Balance = balance;
        Address = address;
    }
}
