﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using N3.CqrsEs.Ramverk;
using N3.CqrsEs.SkrivModell.Kommando;
using N3.CqrsEs.SkrivModell.KommandoHantering;
using N3.CqrsEs.SkrivModell.Konfiguration;

namespace N3.CqrsEs.SkrivModell
{
    public static class HostExtensions
    {
        /// <summary>
        /// Följande tjänter måste konfigureras hos konsumenten.
        /// <see cref="IÄrendeNummerUträknare"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration">Se möjliga inställningar genom att titta här: <see cref="N3SkrivModellKonfiguration"/></param>
        /// <returns></returns>
        public static IServiceCollection LäggTillSkrivModell(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            return services
                .Configure<N3SkrivModellKonfiguration>(configuration)
                .AddScoped<
                    IKommandoHanterare<SkapaInkassoÄrendeKommando>,
                    InkassoÄrendeKommandoHanterare
                >()
                .AddScoped<
                    IKommandoHanterare<TilldelaÄrendeNummerTillInkassoÄrendeKommando>,
                    InkassoÄrendeKommandoHanterare
                >();
        }
    }
}
