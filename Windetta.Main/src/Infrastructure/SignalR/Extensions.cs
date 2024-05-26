using Microsoft.AspNetCore.SignalR;

namespace Windetta.Main.Infrastructure.SignalR;

public record MirrorData<T>(string method, T data);

public static class Extensions
{
    public static Task SendToMirrorAsync<T>(this IClientProxy client, MirrorData<T> data, CancellationToken token = default)
    {
        return client.SendAsync("mirror", data, token);
    }
}
