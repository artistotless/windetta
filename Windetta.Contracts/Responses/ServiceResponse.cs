namespace Windetta.Contracts.Responses;

public class ServiceResponse
{
    public bool Success => Error is null;

    public string? Error { get; set; }
}
