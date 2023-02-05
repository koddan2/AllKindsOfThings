
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SAK.Deser;

public static class JsonExtensions
{
    private static readonly JsonSerializerOptions _DefaultPretty = new() { WriteIndented = true };

    public static string ToJson<T>(this T obj, JsonSerializerOptions serializerOptions) =>
        JsonSerializer.Serialize(obj, serializerOptions);

    public static string ToJson<T>(this T obj) => JsonSerializer.Serialize(obj);

    public static string ToJson<T>(this T obj, bool prettyPrint) =>
        prettyPrint
            ? JsonSerializer.Serialize(obj, _DefaultPretty)
            : obj.ToJson();
}