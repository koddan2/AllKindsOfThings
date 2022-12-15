namespace DZT.Lib.Helpers;

public static class EnumerableExtensions
{
    public static IEnumerable<T> SideEffect<T>(this IEnumerable<T> source, Action<T> sideEffect)
    {
        foreach (var item in source
        )
        {
            sideEffect.Invoke(item);
            yield return item;
        }
    }

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