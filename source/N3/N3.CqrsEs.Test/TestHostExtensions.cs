using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using N3.CqrsEs.LäsModell.Infrastruktur;
using N3.CqrsEs.Ramverk;
using N3.CqrsEs.SkrivModell.KommandoHantering;
using N3.CqrsEs.Test.TestTjänster;

namespace N3.CqrsEs.Test
{
    public static class TestHostExtensions
    {
        public static IServiceCollection RegistreraTestTjänster(this IServiceCollection services)
        {
            return services
                .AddScoped<TestVyLagringDatabas>()
                .AddScoped<IVyLagring, TestVyLagring>()
                .AddScoped<IÄrendeNummerUträknare, TestVyLagring>()
                .AddScoped<IHändelseKassa, MinnesBaseradHändelseKassa>();
        }

        public static T Plocka<T>(this IServiceScope scope)
            where T : notnull
        {
            return scope.ServiceProvider.GetRequiredService<T>();
        }
    }
}
