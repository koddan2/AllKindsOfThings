namespace DZT.Lib.Helpers;

public static class StringExtensions
{
    public static bool IsNullOrEmpty(this string? s) => string.IsNullOrEmpty(s);
    public static int? ParseInt(this string str)
    {
        if (int.TryParse(str, out int result))
        {
            return result;
        }
        return null;
    }
}
