namespace Windetta.Common.Authentication;

public abstract class JsonWebTokenBase
{
    public Guid Id { get; set; }
    public string AccessToken { get; set; }
    public long Expires { get; set; }
    public string RefreshToken { get; set; }
}
