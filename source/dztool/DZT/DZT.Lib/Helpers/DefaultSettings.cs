using System.Text.Json;

namespace DZT.Lib.Helpers;

public class DefaultSettings
{
    public static JsonSerializerOptions JsonSerializerOptions { get; set; } = new JsonSerializerOptions { WriteIndented = true };
}
