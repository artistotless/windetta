namespace Windetta.Common.Authentication;

public class AccessToken
{
    public Guid UserId { get; set; }
    public string Payload { get; set; }
    public long Expires { get; set; }
}
