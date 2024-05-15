namespace Windetta.Common.Types;

public class BaseResponse<T> : BaseResponse
{
    public T Data { get; set; }

    public BaseResponse(T data)
    {
        Data = data;
        Success = true;
    }
}

public class BaseResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }

    public BaseResponse(bool success, string? error)
    {
        Success = success;
        Error = error;
    }

    public BaseResponse()
    {

    }
}