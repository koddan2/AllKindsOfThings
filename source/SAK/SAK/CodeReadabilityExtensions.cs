using System.Globalization;
namespace SAK;

public static class CodeReadabilityExtensions
{
    public static double? AsInt(this string str, CultureInfo? cultureInfo)
    {
        return int.TryParse(str, cultureInfo ?? CultureInfo.InvariantCulture, out int result) ? result : null;
    }

    public static double? AsDouble(this string str, CultureInfo? cultureInfo)
    {
        return double.TryParse(str, cultureInfo ?? CultureInfo.InvariantCulture, out double result) ? result : null;
    }
}