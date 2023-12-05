using Windetta.Common.Constants;
using Windetta.Common.Types;

namespace Windetta.Main.Core.Exceptions;

public class MatchHubException : WindettaException
{
    public new string? Message { get; set; }

    public static MatchHubException AlreadyMemberOfHub
        => new MatchHubException(Errors.Main.AlreadyMemberOfHub);

    public static MatchHubException BetValidationFail
        => new MatchHubException(Errors.Main.BetValidationFail);

    public static MatchHubException NotFound
        => new MatchHubException(Errors.Main.MatchHubNotFound);

    public static MatchHubException BetIsLessMinimumAllowed
        => new MatchHubException(Errors.Main.BetIsLessMinimumAllowed);

    private MatchHubException(string errorCode) : base(errorCode) { }
}
