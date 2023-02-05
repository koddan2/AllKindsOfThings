using System.Globalization;
namespace SAK;

public static class CodeReadabilityExtensions
{
    public static int? AsInt(this string str, CultureInfo? cultureInfo)
    {
        return int.TryParse(str, cultureInfo ?? CultureInfo.InvariantCulture, out int result) ? result : null;
    }

    public static float? AsFloat(this string str, CultureInfo? cultureInfo)
    {
        return float.TryParse(str, cultureInfo ?? CultureInfo.InvariantCulture, out float result) ? result : null;
    }

    public static double? AsDouble(this string str, CultureInfo? cultureInfo)
    {
        return double.TryParse(str, cultureInfo ?? CultureInfo.InvariantCulture, out double result) ? result : null;
    }

    public static decimal? AsDecimal(this string str, CultureInfo? cultureInfo)
    {
        return decimal.TryParse(str, cultureInfo ?? CultureInfo.InvariantCulture, out decimal result) ? result : null;
    }
}