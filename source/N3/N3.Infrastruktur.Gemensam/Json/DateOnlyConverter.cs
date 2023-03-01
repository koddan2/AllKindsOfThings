using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace N3.App.Domän.Api.Web.Vanligt
{
    public partial class DateOnlyConverter : JsonConverter<DateOnly>
    {
        public override DateOnly Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            if (reader.TryGetDateTime(out var dt))
            {
                return DateOnly.FromDateTime(dt);
            }
            ;

            var value = reader.GetString();
            if (value == null)
            {
                return default;
            }
            var match = DateOnlyRegex().Match(value);
            return match.Success
                ? new DateOnly(
                    int.Parse(match.Groups[1].Value),
                    int.Parse(match.Groups[2].Value),
                    int.Parse(match.Groups[3].Value)
                )
                : default;
        }

        public override void Write(
            Utf8JsonWriter writer,
            DateOnly value,
            JsonSerializerOptions options
        ) => writer.WriteStringValue(value.ToString("yyyy-MM-dd"));

        [GeneratedRegex("^(\\d\\d\\d\\d)-(\\d\\d)-(\\d\\d)(T|\\s|\\z)")]
        internal static partial Regex DateOnlyRegex();
    }
}
