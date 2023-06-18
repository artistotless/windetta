using System.Text.Json;

namespace Windetta.Common.Authentication;

public class JsonWebToken : JsonWebTokenBase
{
    public JsonWebToken(AccessToken accessToken, string refreshToken)
    {
        AccessToken = accessToken.Payload;
        RefreshToken = refreshToken;
        Expires = accessToken.Expires;
        Id = accessToken.UserId;
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
