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

    public static LobbyException MemberAreNotInRoom
    => new LobbyException(Errors.Main.MemberAreNotInRoom);

    public static LobbyException BetIsLessMinimumAllowed
        => new LobbyException(Errors.Main.BetIsLessMinimumAllowed);

    public static LobbyException TeamsAreLessMinimumRequired
        => new LobbyException(Errors.Main.TeamsAreLessMinimumRequired);

    public static LobbyException TeamsAreGreaterMaximumAllowed
        => new LobbyException(Errors.Main.TeamsAreGreaterMaximumAllowed);

    public static LobbyException SlotsAreLessMinimumRequired
        => new LobbyException(Errors.Main.SlotsAreLessMinimumRequired);

    public static LobbyException SlotsAreGreaterMaximumAllowed
        => new LobbyException(Errors.Main.SlotsAreGreaterMaximumAllowed);

    public LobbyException(string errorCode) : base(errorCode) { }
}
