using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using N3.CqrsEs.Gemensam.Händelser;
using N3.CqrsEs.LäsModell.HändelseHantering;
using N3.CqrsEs.LäsModell.Konfiguration;

namespace N3.CqrsEs.LäsModell
{
    public static class HostExtensions
    {
        public static IServiceCollection LäggTillLäsModell(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            return services
                .Configure<N3LäsModellKonfiguration>(configuration)
                .AddScoped<
                    IHändelseMottagare<InkassoÄrendeBlevTilldelatÄrendeNummer>,
                    InkassoÄrendeLäsDatabasKontext
                >()
                .AddScoped<
                    IHändelseMottagare<InkassoÄrendeSkapades>,
                    InkassoÄrendeLäsDatabasKontext
                >();
        }
    }
}
