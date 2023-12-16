using Windetta.Common.Constants;
using Windetta.Common.Types;

namespace Windetta.Main.Core.Exceptions;

public class LspmException : WindettaException
{
    public new string? Message { get; set; }

    public static LspmException NotFound
        => new LspmException(Errors.Main.LspmNotFound); 
    public static LspmException Overload
        => new LspmException(Errors.Main.LspmOverload);

    private LspmException(string errorCode) : base(errorCode) { }
}