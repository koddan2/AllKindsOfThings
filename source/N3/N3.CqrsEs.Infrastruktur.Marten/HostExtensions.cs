using Marten;
using Marten.Events;
using Marten.Schema.Identity;
using Marten.Services;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using N3.CqrsEs.Ramverk;
using N3.CqrsEs.SkrivModell.Domän;
using SAK;
using System.Data.Common;
using System.Text.Json;
using Weasel.Core;

namespace N3.CqrsEs.Infrastruktur.Marten
{
    public static class HostExtensions
    {
        public static IServiceCollection LäggTillCqrsEsInfrastrukturMarten(
            this IServiceCollection services,
            IConfiguration configuration,
            IHostEnvironment hostEnvironment
        )
        {
            _ = services
                .AddScoped<IHändelseKassa, MartenHändelseKassa>()
                .AddScoped<AggregateRepository>()
                ////.Configure<JsonSerializerOptions>(opts =>
                ////{
                ////    opts.WriteIndented = true;
                ////})
                ////.Configure<SystemTextJsonSerializer>(serializer =>
                ////{
                ////    serializer.EnumStorage = EnumStorage.AsString;
                ////    serializer.Customize(opts =>
                ////    {
                ////        opts.WriteIndented = true;
                ////    });
                ////})
                .AddMarten(options =>
                {
                    // Establish the connection string to your Marten database
                    var connString = configuration.GetConnectionString("Marten").OrFail();
                    options.Connection(connString);

                    Anpassa(configuration, options);

                    // If we're running in development mode, let Marten just take care
                    // of all necessary schema building and patching behind the scenes
                    if (hostEnvironment.IsDevelopment())
                    {
                        options.AutoCreateSchemaObjects = AutoCreate.All;
                    }
                });

            return services;
        }

        private static void Anpassa(IConfiguration configuration, StoreOptions options)
        {
            options.Events.StreamIdentity = StreamIdentity.AsString;
            ////options.Events.MetadataConfig.HeadersEnabled = true;
            options.Events.MetadataConfig.CausationIdEnabled = true;
            options.Events.MetadataConfig.CorrelationIdEnabled = true;
            _ = options.Policies.ForAllDocuments(mapping =>
            {
                mapping.Metadata.DotNetType.Enabled = true;
            });

            var ser = new SystemTextJsonSerializer { EnumStorage = EnumStorage.AsString };
            ser.Customize(opts =>
            {
                opts.WriteIndented = true;
            });
            options.Serializer(ser);

            // instead of
            ////options.Serializer<SystemTextJsonSerializer>();

            if (configuration["SchemaName"] is string schemaName)
            {
                options.DatabaseSchemaName = schemaName;
            }
        }
    }
}
