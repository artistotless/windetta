using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace Windetta.Main.Infrastructure.Sagas;

public class PropertiesDbModelConverter : ValueConverter<IReadOnlyDictionary<string, string>, string>
{
    private static string ToString(IReadOnlyDictionary<string, string> properties)
        => JsonSerializer.Serialize(properties);

    private static IReadOnlyDictionary<string, string> ToDict(string serializedData)
     => JsonSerializer.Deserialize<IReadOnlyDictionary<string, string>>(serializedData)!;

    public PropertiesDbModelConverter()
        : base(
            v => ToString(v),
            v => ToDict(v))
    {

    }
}
