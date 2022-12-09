using System.Globalization;
namespace SAK;

public static class FluencyForNumbers
{
    public static double? AsDouble(this string str)
    {
        return double.TryParse(str, CultureInfo.InvariantCulture, out double result) ? result : null;
    }
}