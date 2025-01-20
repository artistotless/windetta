using LSPM.Core.Interfaces;
using LSPM.Models;
using System.Text;
using System.Text.Json;

namespace LSPM.Infrastructure.Services;

/// <inheritdoc cref="IGameServerMatchClient"/>
public class GameServerMatchClient : IGameServerMatchClient
{
    private readonly IHttpClientFactory _clientFactory;

    private const string _clientName = "match-client";

    public GameServerMatchClient(IHttpClientFactory factory)
    {
        _clientFactory = factory;
    }

    public async Task CancelMatchAsync(Uri serverEndpoint, Guid matchId)
    {
        var client = _clientFactory.CreateClient(_clientName);

        var matchesEndpoint = new Uri(serverEndpoint, $"api/matches/{matchId}");

        await client.DeleteAsync(matchesEndpoint);
    }

    public async Task CreateMatchAsync(Uri serverEndpoint, MatchInitializationData data)
    {
        var client = _clientFactory.CreateClient(_clientName);

        var matchesEndpoint = new Uri(serverEndpoint, "api/matches");

        var content = new StringContent(
                JsonSerializer.Serialize(data),
                Encoding.UTF8,
                "application/json");

        await client.PostAsync(matchesEndpoint, content);
    }
}