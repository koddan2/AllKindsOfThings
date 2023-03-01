////using SAK;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using N3.CqrsEs.Ramverk;

namespace N3.Låtsas
{
    public static class HostExtensions
    {
        public static IServiceCollection InstalleraLåtsasTjänster(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            return services
                .Configure<LåtsasAktivitetsBussKonfiguration>(configuration)
                .AddScoped<IAktivitetsBuss, LåtsasAktivitetsBuss>();
        }
    }
}
