
using Urdep.Extensions.Augmentation;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace SAK.Deser;

public static class JsonExtensions
{
    private static readonly JsonSerializerOptions _DefaultPretty = new() { WriteIndented = true };

    public static string ToJson<T>(this IAugmented<T> obj, JsonSerializerOptions serializerOptions) =>
        JsonSerializer.Serialize(obj, serializerOptions);

    public static string ToJson<T>(this IAugmented<T> obj) => JsonSerializer.Serialize(obj);

    public static string ToJson<T>(this IAugmented<T> obj, bool prettyPrint) =>
        prettyPrint
            ? JsonSerializer.Serialize(obj, _DefaultPretty)
            : obj.ToJson();
}

/// <summary>
/// See https://stackoverflow.com/a/73583938
/// <example>
/// <code>
/// JsonSerializerOptions options = new JsonSerializerOptions
/// {
/// 	TypeInfoResolver = new PrivateConstructorContractResolver()
/// };
/// </code>
/// </example>
/// </summary>
public class PrivateConstructorContractResolver : DefaultJsonTypeInfoResolver
{
	public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
	{
		JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);

		if (jsonTypeInfo.Kind == JsonTypeInfoKind.Object && jsonTypeInfo.CreateObject is null)
		{
			if (jsonTypeInfo.Type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Length == 0)
			{
				// The type doesn't have public constructors
				jsonTypeInfo.CreateObject = () =>
					Activator.CreateInstance(jsonTypeInfo.Type, true)
						?? throw new InvalidOperationException($"Could not instantiate type {type}");
			}
		}

		return jsonTypeInfo;
	}
}