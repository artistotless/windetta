namespace Windetta.Common.Middlewares;

internal class ErrorDto
{
    public string ErrorCode { get; set; }
    public string Message { get; set; }

    public ErrorDto(string errorCode, string message)
    {
        ErrorCode = errorCode;
        Message = message;
    }
}
