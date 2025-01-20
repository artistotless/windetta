using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace Windetta.Common.Host;

public static class HostExtensions
{
    private static readonly string MutexPrefix = "Windetta.Mutext";

    public static IHost UseOnlySingleInstanceLaunching(this WebApplication host)
    {
        var mutex = new Mutex(false, $"{MutexPrefix}-{host.Environment.ApplicationName}");

        // Проверка, существует ли уже работающий экземпляр приложения.
        if (!mutex.WaitOne(TimeSpan.Zero, true))
        {
            Console.WriteLine("Application is already running.");
            Environment.Exit(1);
        }

        var lifetime = (IHostApplicationLifetime)host.Services.GetService(typeof(IHostApplicationLifetime));

        // Даем контроль потоку, чтобы если приложение будет завершено, мьютекс освободился.
        lifetime.ApplicationStopping.Register(() => mutex.ReleaseMutex());

        return host;
    }
}
