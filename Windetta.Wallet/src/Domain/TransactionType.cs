namespace Windetta.Wallet.Domain;

public enum TransactionType
{
    TopUp,
    Withdrawal,
    CancelWithdrawal,
    TransferOut,
    TransferIn,
    Loss,
    Winnings
}