using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace Windetta.Main.Infrastructure.Sagas;

public class TicketsDbModelConverter : ValueConverter<IReadOnlyDictionary<Guid, string>, string>
{
    private static string ToString(IReadOnlyDictionary<Guid, string> tickets)
        => JsonSerializer.Serialize(tickets);

    private static IReadOnlyDictionary<Guid, string> ToDict(string serializedData)
     => JsonSerializer.Deserialize<IReadOnlyDictionary<Guid, string>>(serializedData)!;

    public TicketsDbModelConverter()
        : base(
            v => ToString(v),
            v => ToDict(v))
    {

    }
}
