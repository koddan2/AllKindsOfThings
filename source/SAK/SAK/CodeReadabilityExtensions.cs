using System.Globalization;
namespace SAK;

public static class CodeReadabilityExtensions
{
    private static readonly object _Lock = new();
    private static CultureInfo _CultureInfo = CultureInfo.InvariantCulture;
    public static CultureInfo DefaultCulture
    {
        get => _CultureInfo;
        set
        {
            lock (_Lock)
            {
                _CultureInfo = value;
            }
        }
    }

    public static int? AsInt(this string str, CultureInfo? cultureInfo)
    {
        return int.TryParse(str, cultureInfo ?? DefaultCulture, out int result) ? result : null;
    }

    public static float? AsFloat(this string str, CultureInfo? cultureInfo)
    {
        return float.TryParse(str, cultureInfo ?? DefaultCulture, out float result) ? result : null;
    }

    public static double? AsDouble(this string str, CultureInfo? cultureInfo)
    {
        return double.TryParse(str, cultureInfo ?? DefaultCulture, out double result) ? result : null;
    }

    public static decimal? AsDecimal(this string str, CultureInfo? cultureInfo)
    {
        return decimal.TryParse(str, cultureInfo ?? DefaultCulture, out decimal result) ? result : null;
    }

    public static bool? AsBool(this string str)
    {
        return bool.TryParse(str, out bool result) ? result : null;
    }
}