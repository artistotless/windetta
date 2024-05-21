using Microsoft.AspNetCore.SignalR;
using Windetta.Common.Types;

namespace Windetta.Main.Infrastructure.SignalR;

public class SubjectClaimUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst(JwtClaimTypes.Subject)?.Value!;
    }
}
