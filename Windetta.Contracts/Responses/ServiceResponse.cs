namespace Windetta.Contracts.Responses;

public class ServiceResponse
{
    public bool Success => Error is null && ErrorCode is null;

    public string? Error { get; set; }
    public string? ErrorCode { get; set; }
}