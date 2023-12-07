using Windetta.Common.Constants;
using Windetta.Common.Types;

namespace Windetta.Main.Core.Exceptions;

public class MatchHubPluginException : WindettaException
{
    public new string? Message { get; set; }

    public static MatchHubPluginException RequirementTypeNotAllowed
        => new MatchHubPluginException(Errors.Main.RequirementTypeNotAllowed);

    public static MatchHubPluginException JoinFilterValidationFail
        => new MatchHubPluginException(Errors.Main.JoinFilterValidationFail);

    public static MatchHubPluginException InvalidPluginName
        => new MatchHubPluginException(Errors.Main.InvalidPluginName);

    public static MatchHubPluginException RequiredValuesNotProvided
        => new MatchHubPluginException(Errors.Main.RequiredValuesNotProvided);

    public static MatchHubPluginException RequirementValueInvalid
        => new MatchHubPluginException(Errors.Main.RequirementValueInvalid);

    private MatchHubPluginException(string errorCode) : base(errorCode) { }
}