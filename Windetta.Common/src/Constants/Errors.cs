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
        public const string BigFeeTransaction = "big_fee_transaction";
        public const string NotFound = "wallet_notfound";
    }
}
