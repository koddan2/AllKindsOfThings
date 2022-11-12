namespace DZT.Lib.Helpers;

public static class StringExtensions
{
    public static int? ParseInt(this string str)
    {
        if (int.TryParse(str, out int result))
        {
            return result;
        }
        return null;
    }
}
