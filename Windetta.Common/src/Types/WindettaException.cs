namespace Windetta.Common.Types;

public class WindettaException : Exception
{
    public string ErrorCode { get; set; }

    public WindettaException(string errorCode)
    {
        ErrorCode = errorCode.ToLower();
    }

    public WindettaException(string errorCode, string message) : base(message)
    {
        ErrorCode = errorCode.ToLower();
    }
}
