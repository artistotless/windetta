namespace Windetta.Contracts.Responses;

public class ServiceResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }

    public ServiceResponse(bool success, string? error = null)
    {
        Success = success;
        Error = error;
    }

    public static ServiceResponse Successfull => new(true);
}

public class ServiceResponse<TMessage> : ServiceResponse
{
    public ServiceResponse(bool success, TMessage message, string? error = null) : base(success, error)
    {
        Message = message;
    }

    public TMessage Message { get; set; }

}