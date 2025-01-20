using LSPM.Core.Interfaces;
using LSPM.Core.Models;
using LSPM.Core.Services;
using System.Diagnostics;

namespace LSPM.Infrastructure.Services;

public class GameServerLauncher : IGameServerLauncher
{
    private readonly IGameServersOptions _options;
    private readonly EndpointResolver _endpointResolver;

    public GameServerLauncher(IGameServersOptions options, EndpointResolver endpointResolver)
    {
        _options = options;
        _endpointResolver = endpointResolver;
    }

    /// <summary>
    /// Runs the game server of a specific game
    /// </summary>
    /// <returns>Endpoint for connecting players</returns>
    public async ValueTask<GameServerInfo> LaunchAsync(Guid gameId)
    {
        var instanceId = Guid.NewGuid();
        var options = _options.Get(gameId);
        var gameServerIp = await _endpointResolver.GetExternalIp();
        var ipcHttpPort = _endpointResolver.GetNewHttpPort();
        var protocolPort = options.NeedProtocolPort
            ? _endpointResolver.GetNewProtocolPort() : ipcHttpPort;

        var process = CreateProcess($"{options.RunTool}");

        process.StartInfo.ArgumentList.Add($"{options.Path}/{options.ExecutableFile}");
        process.StartInfo.ArgumentList.Add("--id"); process.StartInfo.ArgumentList.Add(instanceId.ToString());
        process.StartInfo.ArgumentList.Add("--lspm"); process.StartInfo.ArgumentList.Add(HostConfig.Endpoint);
        process.StartInfo.ArgumentList.Add("--ipcPort"); process.StartInfo.ArgumentList.Add(ipcHttpPort.ToString());
        process.StartInfo.ArgumentList.Add("--directory"); process.StartInfo.ArgumentList.Add(options.Path);
        process.StartInfo.ArgumentList.Add("--port"); process.StartInfo.ArgumentList.Add(protocolPort.ToString());

        var endpoint = new Uri(string.Format(options.EndpointFormat, options.Protocol, gameServerIp, protocolPort));

        try
        {
            process.Start();
        }
        catch
        {
            throw new Exception
                ($"Cannot start a process. GameId: {gameId}");
        }

        return new GameServerInfo()
        {
            Endpoint = endpoint,
            InstanceId = instanceId,
            IpcEndpoint = new Uri($"http://localhost:{ipcHttpPort}")
        };
    }

    private static Process CreateProcess(string path)
    {
        var serverProcess = new Process();
        serverProcess.StartInfo.UseShellExecute = true;
        serverProcess.StartInfo.FileName = path;
        serverProcess.StartInfo.CreateNoWindow = false;
        serverProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;

        return serverProcess;
    }
}