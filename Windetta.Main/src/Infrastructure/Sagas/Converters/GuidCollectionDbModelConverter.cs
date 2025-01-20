using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace Windetta.Main.Infrastructure.Sagas.Converters;

public class GuidCollectionDbModelConverter : ValueConverter<IEnumerable<Guid>, string>
{
    private static string ToString(IEnumerable<Guid> players)
     => JsonSerializer.Serialize(players);

    private static IEnumerable<Guid> ToCollection(string serializedData)
     => JsonSerializer.Deserialize<IEnumerable<Guid>>(serializedData)!;

    public GuidCollectionDbModelConverter()
        : base(
            v => ToString(v),
            v => ToCollection(v))
    {

    }
}