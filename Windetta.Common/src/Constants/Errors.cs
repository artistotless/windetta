namespace Windetta.Common.Constants;

public static class Errors
{
    public static class Main
    {
        #region Room
        public const string MemberAlreadyJoined = "member_already_joined";
        public const string MemberNotInRoom = "member_not_in_room";
        public const string MaxMembersInRoomReached = "maxmembers_in_room_reached";
        public const string JoinFilterValidationFail = "join_filter_validation_fail";
        public const string CannotCreateMoreThanOneHub = "cannot_create_more_than_one_hub";
        #endregion
    }

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
