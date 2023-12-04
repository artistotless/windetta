using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Windetta.Common.Configuration;

namespace Windetta.Common.Mongo;

public static class Extensions
{
    public static void AddMongo(this IServiceCollection services)
    {
        BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        var provider = services.BuildServiceProvider();
        var cfg = provider.GetRequiredService<IConfiguration>();

        var options = cfg.GetOptions<MongoDbOptions>("MongoDb");
        services.Configure<MongoDbOptions>(cfg.GetSection("MongoDb"));

        services.AddSingleton<IMongoClient, MongoClient>((p) => new MongoClient(options.ConnectionURI));
    }
}
