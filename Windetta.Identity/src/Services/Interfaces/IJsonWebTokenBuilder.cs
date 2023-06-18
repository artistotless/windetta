using System.Security.Claims;
using Windetta.Common.Authentication;
using Windetta.Common.Types;

namespace Windetta.Identity.Services;

public interface IJsonWebTokenBuilder : IScopedService
{
    Task<JsonWebTokenBase> BuildAsync(Guid userId, IDictionary<string, string> claims);
}
