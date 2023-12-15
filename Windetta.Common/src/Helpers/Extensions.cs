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

    public static string Underscore(this string value)
      => string.Concat(value.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString()));

    public static string ParseIdentifier(this Uri value)
        => $"{value.DnsSafeHost}{value.Port}".Replace(".", string.Empty);
}
