using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Ncs.Explore.Cli;

internal static class ExploreExtensions
{
	private static readonly JsonSerializerOptions _JsonSerializerOptions = new() { WriteIndented = true };
	public static string ToJson<T>(this T instance)
	{
		return JsonSerializer.Serialize(instance, _JsonSerializerOptions);
	}

	public static IEnumerable<KeyValuePair<string, string?>> GetSubSectionOnly(this IConfiguration configuration, string sectionName)
	{
		return configuration
			.GetSection(sectionName)
			.AsEnumerable()
			.ToDictionary(x => x.Key.Replace($"{sectionName}:", ""), y => y.Value)
			.AsEnumerable();
	}
}
