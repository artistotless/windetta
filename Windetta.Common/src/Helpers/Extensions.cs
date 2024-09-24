using Microsoft.Extensions.Primitives;

namespace Windetta.Common.Helpers;

public static class Extensions
{
    public static string Cut(this string text, int maxLength)
    {
        if (text.Length <= maxLength)
            return text; // No need to cut, return the original string
        else
            return text.Substring(0, maxLength);
    }

    public static string Cut(this Guid id, int count)
    {
        const int startIndex = 9;

        var text = id.ToString().Replace("-", string.Empty);

        if ((startIndex + count) > text.Length)
            throw new ArgumentException("count too large");

        return text.Substring(startIndex, count);
    }

    public static string Underscore(this string value)
      => string.Concat(value.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString()));

    public static string? ToNullableString(this string value)
        => (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value)) ? null : value;

    public static string? ToNullableString(this StringValues value) => ToNullableString(value.ToString());
}
