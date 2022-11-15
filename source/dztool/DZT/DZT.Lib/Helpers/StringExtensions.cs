namespace DZT.Lib.Helpers;

public static class LinqExtensions
{
    public static IDictionary<A, B> Mutate<A, B>(this IDictionary<A, B> dict, Func<A, B, B> mutator)
    {
        var keys = dict.Keys;
        foreach (var key in keys)
        {
            dict[key] = mutator(key, dict[key]);
        }

        return dict;
    }
}
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
