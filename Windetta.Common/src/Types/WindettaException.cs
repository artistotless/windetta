namespace Windetta.Common.Types;

public class WindettaException : Exception
{
    public string ErrorCode { get; set; }

    public WindettaException(string? message, string errorCode) : base(message)
    {
        ErrorCode = errorCode.ToLower();
    }
}
