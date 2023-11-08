namespace Windetta.Common.Constants;

public static class Errors
{
    public static class Identity
    {
        public const string UserNotFound = "user_not_found";
        public const string AuthCodeNotFound = "auth_code_not_found";
    }
    public static class Wallet
    {
        public const string InvalidFreezeAmount = "invalid_freeze_amount";
        public const string FundsNotEnough = "funds_not_enough";
        public const string FundsAlreadyHeld = "funds_already_held";
        public const string NotFound = "wallet_notfound";
        public const string BalanceNotFound = "wallet_balance_notfound";
    }

    public static class TonTxns
    {
        public const string TonTransferError = "ton_transfer_error";
    }
}
