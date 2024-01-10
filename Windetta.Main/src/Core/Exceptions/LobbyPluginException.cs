using Windetta.Common.Constants;
using Windetta.Common.Types;

namespace Windetta.Main.Core.Exceptions;

public class LobbyPluginException : WindettaException
{
    public new string? Message { get; set; }

    public static LobbyPluginException RequirementTypeNotAllowed
        => new LobbyPluginException(Errors.Main.RequirementTypeNotAllowed);

    public static LobbyPluginException JoinFilterValidationFail
        => new LobbyPluginException(Errors.Main.JoinFilterValidationFail);

    public static LobbyPluginException InvalidPluginName
        => new LobbyPluginException(Errors.Main.InvalidPluginName);

    public static LobbyPluginException RequiredValuesNotProvided
        => new LobbyPluginException(Errors.Main.RequiredValuesNotProvided);

    public static LobbyPluginException RequirementValueInvalid
        => new LobbyPluginException(Errors.Main.RequirementValueInvalid);

    private LobbyPluginException(string errorCode) : base(errorCode) { }
}