using LSPM.Core.Exceptions;
using LSPM.Core.Models;
using Microsoft.Extensions.Options;

namespace LSPM.Core.Services;

public sealed class EndpointResolver
{
    private string? _localIp;
    private string? _externalIp;

    private int _httpPort;
    private int _protocolPort;

    public PortRange HttpPortRange { get; set; }
    public PortRange ProtocolPortRange { get; set; }

    private readonly EndpointResolvingOptions _options;

    public EndpointResolver(IOptions<EndpointResolvingOptions> options)
    {
        _options = options.Value;

        _httpPort = _options.HttpPortStart;
        _protocolPort = _options.ProtocolPortStart;

        HttpPortRange = new PortRange
            (_options.HttpPortStart,
            _options.HttpPortStart + _options.MaxExposePorts);

        ProtocolPortRange = new PortRange
            (_options.ProtocolPortStart,
            _options.ProtocolPortStart + _options.MaxExposePorts);

        _localIp = GetLocalIpFromConfiguration();
        _localIp = GetLocalIpFromEnv();

        _externalIp = GetExternalIpFromConfiguration();
        _externalIp = GetExternalIpFromEnv();
    }

    public string GetLocalIP()
    {
        return _localIp ?? throw new Exception
            ("Couldn't figure out the local Ip");
    }

    public async ValueTask<string> GetExternalIp()
    {
        if (_externalIp is null)
            _externalIp = await GetExternalIpFromWeb();

        return _externalIp ?? throw new Exception
            ("Couldn't figure out the extenal Ip");
    }

    public int GetNewHttpPort()
    {
        if (_httpPort > HttpPortRange.End)
            throw LspmException.Overload.WithError
            ("All http ports are exhausted");

        var port = Interlocked.Increment(ref _httpPort);

        return port;
    }

    public int GetNewProtocolPort()
    {
        if (_protocolPort > ProtocolPortRange.End)
            throw LspmException.Overload.WithError
            ("All protocol ports are exhausted");

        var port = Interlocked.Increment(ref _protocolPort);

        return port;
    }

    private async Task<string?> GetExternalIpFromWeb()
    {
        var client = new HttpClient();
        var cancellation = new CancellationTokenSource
            (_options.FetchIpTimeout);

        try
        {
            return await client
                .GetStringAsync(_options.FetchIpApi, cancellation.Token);
        }
        catch { return _externalIp; }
        finally { client.Dispose(); }
    }

    private string? GetLocalIpFromEnv()
        => Environment.GetEnvironmentVariable("localIp") ?? _localIp;

    private string? GetExternalIpFromEnv()
    => Environment.GetEnvironmentVariable("externalIp") ?? _externalIp;

    private string? GetLocalIpFromConfiguration()
       => string.IsNullOrEmpty(_options.LocalIp) ? _localIp : _options.LocalIp;

    private string? GetExternalIpFromConfiguration()
   => string.IsNullOrEmpty(_options.ExternalIp) ? _externalIp : _options.ExternalIp;
}