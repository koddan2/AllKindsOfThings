namespace DZT.Lib.Helpers;

public class CategorizedDouble
{
    private readonly double _minimal;
    private readonly double _small;
    private readonly double _medium;
    private readonly double _large;
    private readonly double _max;

    public CategorizedDouble(
        double minimal = 0,
        double small = 0,
        double medium = 0,
        double large = 0,
        double max = 1
    )
    {
        _minimal = minimal;
        _small = small;
        _medium = medium;
        _large = large;
        _max = max;
    }

    public double Get(CategoryValue category, double modifier = 0d)
    {
        var v = category switch
        {
            CategoryValue.Minimal => _minimal,
            CategoryValue.Small => _small,
            CategoryValue.Medium => _medium,
            CategoryValue.Large => _large,
            CategoryValue.Max => _max,
            _ => default,
        };
        return Math.Clamp(v + modifier, 0, 1);
    }
}
