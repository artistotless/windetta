namespace Windetta.Common.Constants;

public static class Errors
{
    public static class Main
    {
        #region MatchHub
        public const string MemberAlreadyJoinedRoom = "member_already_joined_room";
        public const string MemberNotInRoom = "member_not_in_room";
        public const string MaxMembersInRoomReached = "maxmembers_in_room_reached";
        public const string BetValidationFail = "bet_validation_fail";
        public const string AlreadyMemberOfHub = "already_member_of_hub";
        public const string MatchHubNotFound = "matchhub_notfound";
        public const string BetIsLessMinimumAllowed = "bet_is_less_minimum_allowed";
        #endregion

        #region MatchHubPlugin
        public const string InvalidPluginName = "invalid_plugin_name";
        public const string RequirementTypeNotAllowed = "requirement_type_not_allowed";
        public const string JoinFilterValidationFail = "join_filter_validation_fail";
        public const string RequiredValuesNotProvided = "required_values_not_provided";
        public const string RequirementValueInvalid = "requirement_value_invalid";
        #endregion

        #region Game
        public const string GameConfigurationsNotFound = "game_configurations_notfound";
        #endregion

        #region LSPM
        public const string LspmNotFound = "lspm_notfound";
        public const string LspmOverload = "lspm_overload";
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
