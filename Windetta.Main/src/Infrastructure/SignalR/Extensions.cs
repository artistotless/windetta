using Microsoft.AspNetCore.SignalR;

namespace Windetta.Main.Infrastructure.SignalR;

public static class Extensions
{
    public static Task SendToMirrorAsync(this IClientProxy client, object data, CancellationToken token = default)
    {
        return client.SendAsync("mirror", data, token);
    }
}
