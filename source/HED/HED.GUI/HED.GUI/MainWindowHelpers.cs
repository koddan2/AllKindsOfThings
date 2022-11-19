using System.Drawing;

internal static class AssetHelpers
{
    public static bool TestFont(string fontName)
    {
        float fontSize = 12;
        using Font fontTester = new Font(fontName, fontSize);
        if (fontTester?.Name == fontName)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}