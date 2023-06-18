using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Windetta.Common.Authentication;

public class AccessTokenCreator : IAccessTokenCreator
{
    private readonly JwtOptions _jwtOptions;

    public AccessTokenCreator(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    public AccessToken Create(Guid userId, IDictionary<string, string> claimsKeyValue)
    {
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name,userId.ToString())
        };

        claims.AddRange(claimsKeyValue.Select(x => new Claim(x.Key, x.Value)));

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtOptions.SecretKey);
        var expires = DateTime.UtcNow.AddDays(1);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var payload = tokenHandler.WriteToken(token);

        var result = new AccessToken()
        {
            Expires = ((DateTimeOffset)expires).ToUnixTimeSeconds(), // Represents Unix epoch
            Payload = payload,
            UserId = userId,
        };

        return result;
    }

}
