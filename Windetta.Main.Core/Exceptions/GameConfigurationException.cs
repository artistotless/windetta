using Windetta.Common.Constants;
using Windetta.Common.Types;

namespace Windetta.Main.Core.Exceptions;

public class GameConfigurationException : WindettaException
{
    public static GameConfigurationException NotFound
        => new GameConfigurationException(Errors.Main.GameConfigurationsNotFound);

    private GameConfigurationException(string errorCode) : base(errorCode) { }
}