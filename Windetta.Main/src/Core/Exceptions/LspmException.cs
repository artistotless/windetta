using Windetta.Common.Constants;
using Windetta.Common.Types;

namespace Windetta.Main.Core.Exceptions;

public class LspmException : WindettaException
{
    public new string? Message { get; set; }

    public static LspmException NotFound
        => new LspmException(Errors.LSPM.NotFound);
    public static LspmException Overload
        => new LspmException(Errors.LSPM.Overload);

    private LspmException(string errorCode) : base(errorCode) { }
}