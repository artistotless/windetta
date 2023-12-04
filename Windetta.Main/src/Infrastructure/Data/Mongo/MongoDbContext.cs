using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Windetta.Common.Mongo;
using Windetta.Common.Types;

namespace Windetta.Main.Infrastructure.Data.Mongo;
public class MongoDbContext : ISingletonService
{
    public readonly IMongoCollection<MatchHubDbModel> MatchHubsCollections;

    public MongoDbContext(IOptions<MongoDbOptions> options, IMongoClient client)
    {
        var db = client.GetDatabase(options.Value.DbName);

        MatchHubsCollections = db.GetCollection<MatchHubDbModel>("matchhubs");
    }
}