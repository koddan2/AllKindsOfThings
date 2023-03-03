////using SAK;
using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using N3.CqrsEs.Ramverk;
using SAK;
using Weasel.Core;

namespace N3.Låtsas
{
    public static class HostExtensions
    {
        public static IServiceCollection InstalleraLåtsasTjänster(
            this IServiceCollection services,
            IConfiguration configuration,
            IHostEnvironment hostEnvironment
        )
        {
            if (!hostEnvironment.IsDevelopment() || configuration["test"] is "true")
            {
                throw new ApplicationException(
                    "Cannot use these services in a non-development environment."
                );
            }
            ////_ = services
            ////    .Configure<LåtsasAktivitetsBussKonfiguration>(opts =>
            ////    {
            ////        opts.PostgresConnectionString = configuration.GetConnectionString("Postgres");
            ////    })
            ////    .AddScoped<IAktivitetsBuss, LåtsasAktivitetsBuss>()
            ////.AddMarten(options =>
            ////{
            ////    // This is the absolute, simplest way to integrate Marten into your
            ////    // .Net Core application with Marten's default configuration
            ////    // Establish the connection string to your Marten database
            ////    var connString = configuration.GetConnectionString("Marten").OrFail();
            ////    options.Connection(connString);

            ////    // If we're running in development mode, let Marten just take care
            ////    // of all necessary schema building and patching behind the scenes
            ////    if (hostEnvironment.IsDevelopment())
            ////    {
            ////        options.AutoCreateSchemaObjects = AutoCreate.All;
            ////    }
            ////})
            ;

            return services;
        }
    }
}
