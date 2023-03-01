using System.Text.Json;
using N3.App.Domän.Api.Web.Vanligt;

namespace N3.Infrastruktur.Gemensam.Json
{
    public static class DateOnlyConverterExtensions
    {
        public static void AddDateOnlyConverters(this JsonSerializerOptions options)
        {
            options.Converters.Add(new DateOnlyConverter());
            options.Converters.Add(new DateOnlyNullableConverter());
        }
    }
}
