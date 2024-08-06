using LSPM.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace Windetta.Main.Infrastructure.Sagas.Converters;

public class PlayersDbModelConverter : ValueConverter<IEnumerable<Player>, string>
{
    private static string ToString(IEnumerable<Player> players)
     => JsonSerializer.Serialize(players);

    private static IEnumerable<Player> ToCollection(string serializedData)
     => JsonSerializer.Deserialize<IEnumerable<Player>>(serializedData)!;

    public PlayersDbModelConverter()
        : base(
            v => ToString(v),
            v => ToCollection(v))
    {

    }
}