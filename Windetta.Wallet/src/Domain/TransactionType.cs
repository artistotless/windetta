namespace Windetta.Wallet.Domain;

public enum TransactionType
{
    TopUp,
    Withdrawal,
    CancelDeduct,
    TransferOut,
    TransferIn,
    Loss,
    Winnings
}