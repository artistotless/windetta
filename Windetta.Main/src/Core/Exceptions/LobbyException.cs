using Windetta.Common.Constants;
using Windetta.Common.Types;

namespace Windetta.Main.Core.Exceptions;

public class LobbyException : WindettaException
{
    public new string? Message { get; set; }

    public static LobbyException AlreadyMemberOfLobby
        => new LobbyException(Errors.Main.AlreadyMemberOfLobby);

    public static LobbyException BetValidationFail
        => new LobbyException(Errors.Main.BetValidationFail);

    public static LobbyException NotFound
        => new LobbyException(Errors.Main.LobbyNotFound);

    public static LobbyException BetIsLessMinimumAllowed
        => new LobbyException(Errors.Main.BetIsLessMinimumAllowed);

    private LobbyException(string errorCode) : base(errorCode) { }
}
