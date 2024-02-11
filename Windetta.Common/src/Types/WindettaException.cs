namespace Windetta.Common.Types;

public class WindettaException : Exception
{
    public string ErrorCode { get; set; }
    public new string Message { get; set; }

    public WindettaException(string errorCode)
    {
        ErrorCode = errorCode;
    }

    public WindettaException(string errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
        Message = message;
    }
}
