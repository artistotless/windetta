using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using System.Security.Authentication;
using System.Text;
using Windetta.Common.Types;

namespace Windetta.Common.Authentication;

public static class FakeTokenExtensions
{
    private const string ACCESS_TOKEN = "access_token";

    public static void ConvertAccessTokenToFakeToken(this TicketReceivedContext ticketContext)
    {
        var access_token = ticketContext.Properties?.GetTokenValue(ACCESS_TOKEN);

        if (access_token is null)
            throw new ArgumentNullException(nameof(access_token));

        var payloadPartOfToken = access_token.Split('.')[1];

        switch (payloadPartOfToken.Length % 4)
        {
            case 2: payloadPartOfToken += "=="; break;
            case 3: payloadPartOfToken += "="; break;
        }

        var jsonPayload = Encoding.UTF8.GetString(Convert.FromBase64String(payloadPartOfToken));
        var deserializedPayload = JsonConvert.DeserializeAnonymousType(jsonPayload, new { scope = new string[] { } });

        var userId = ticketContext.Principal?.FindFirst(JwtClaimTypes.Subject)?.Value;
        var nickname = ticketContext.Principal?.FindFirst(JwtClaimTypes.NickName)?.Value;

        if (userId is null)
            throw new AuthenticationException("Required 'userId' from fakeToken is not presented");

        if (nickname is null)
            throw new AuthenticationException("Required 'nickname' from fakeToken is not presented");

        var fakeToken = new FakeToken(Guid.Parse(userId), nickname, deserializedPayload.scope);

        var serializedFakeToken = JsonConvert.SerializeObject(fakeToken);
        var encodedFakeToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(serializedFakeToken));

        if (ticketContext.Properties is null)
            throw new NullReferenceException("Unexpected error. Properties of context's ticket is null.");

        ticketContext.Properties.UpdateTokenValue(ACCESS_TOKEN, encodedFakeToken);
    }
}
