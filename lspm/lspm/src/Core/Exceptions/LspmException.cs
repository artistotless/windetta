using Windetta.Common.Constants;
using Windetta.Common.Types;

namespace LSPM.Core.Exceptions;

public class LspmException : WindettaException
{
    public new string? Message { get; set; }

    public static LspmException Overload
        => new LspmException(Errors.LSPM.Overload);

    public static LspmException LaunchServerTimeout
        => new LspmException(Errors.LSPM.LaunchServerTimeout);

    public static LspmException GameServerNotFound
        => new LspmException(Errors.LSPM.GameServerNotFound);

    public LspmException WithError(string error)
    {
        Message = error;

        return this;
    }

    private LspmException(string errorCode) : base(errorCode) { }
}