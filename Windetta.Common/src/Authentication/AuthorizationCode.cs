namespace Windetta.Common.Authentication;

public class AuthorizationCode
{
    public Guid UserId { get; set; }
    public string Value { get; set; }
    public IDictionary<string, string> Claims { get; set; }
}

