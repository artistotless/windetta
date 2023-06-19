namespace Windetta.Common.Helpers
{
    public static class Extensions
    {
        public static string Cut(this string text, int maxLength)
        {
            if (text.Length <= maxLength)
                return text; // No need to cut, return the original string
            else
                return text.Substring(0, maxLength);
        }
    }
}
