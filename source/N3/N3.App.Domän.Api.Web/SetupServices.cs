using Microsoft.AspNetCore.Http.Json;
using N3.Infrastruktur.Gemensam.Json;
using N3.CqrsEs.Infrastruktur.Marten;
using Newtonsoft.Json.Converters;
using N3.CqrsEs.Messages;
using Rebus.Config;
using N3.App.Domän.Api.Web.Messages;
using Rebus.Routing.TypeBased;
using N3.App.Domän.Api.Web.MessageHandlers;
using N3.CqrsEs;

namespace N3.App.Domän.Api.Web
{
    public static class SetupServices
    {
        public static void AddBasicServices(
            this IServiceCollection services,
            IConfiguration configuration,
            IHostEnvironment hostEnvironment
        )
        {
            _ = services.AddControllers()
            ////.AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new UnikIdentifierareJsonConverter()))
            ;

            if (configuration.GetValue<bool>("EnablePingPong"))
            {
                _ = services.AddHostedService<PingPongPublisherBackgroundService>();
            }
            _ = services.AddMessageBus(configuration);

            _ = services.Configure<JsonOptions>(options =>
            {
                ////options.SerializerOptions.Encoder = null;
                options.SerializerOptions.Converters.Add(new UnikIdentifierareJsonConverter());
                options.SerializerOptions.AddDateOnlyConverters();
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            _ = services.AddEndpointsApiExplorer();

            ////_ = builder.Services.AddSwaggerGen(options =>
            ////{
            ////    options.MapType<UnikIdentifierare>(() => new OpenApiSchema { Type = "string", });
            ////    options.MapType<DateOnly>(() => new OpenApiSchema { Type = "string", });

            ////    options.SwaggerDoc(
            ////        "v1",
            ////        new OpenApiInfo
            ////        {
            ////            Description = "Version 1",
            ////            Title = "Version 1",
            ////            Version = "v1",
            ////        }
            ////    );
            ////});

            ////_ = svc.InstalleraLåtsasTjänster(
            ////    builder.Configuration.GetRequiredSection("Låtsas"),
            ////    builder.Environment
            ////);

            _ = services.LäggTillCqrsEsInfrastrukturMarten(configuration, hostEnvironment);

            _ = services.AddMemoryCache(opts =>
            {
                ////opts.
            });

            _ = services.AddSwaggerDocument(cfg =>
            {
                cfg.ApiGroupNames = new[] { "v1" };
                cfg.SerializerSettings = new Newtonsoft.Json.JsonSerializerSettings()
                {
                    Converters = { new StringEnumConverter(), }
                };
            });
        }

        public static IServiceCollection AddMessageBus(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            _ = services
                .AddRebusHandler<ImporteraInkassoÄrendeMessageHandler>()
                .AddRebus(
                    builder =>
                    {
                        return builder
                            .Transport(
                                transport =>
                                    transport.UsePostgreSql(
                                        configuration.GetConnectionString("RebusPostgres"),
                                        "main_bus",
                                        // receive on this address
                                        inputQueueName: Channels.N3DomainInternal
                                    )
                            )
                            .Subscriptions(
                                sub =>
                                    sub.StoreInPostgres(
                                        configuration.GetConnectionString("RebusPostgres"),
                                        "bus_subscriptions",
                                        isCentralized: true
                                    )
                            )
                            .Routing(
                                r =>
                                    r.TypeBased()
                                        // internal
                                        .Map<ImporteraInkassoÄrendeJobbKommando>(
                                            Channels.N3DomainInternal
                                        )
                                        // common
                                        .Map<PingPongMessage>(Channels.N3DomainCommon)
                                        // common
                                        .Map<ImportAvInkassoÄrendeKölagt>(Channels.N3DomainCommon)
                            );
                    },
                    onCreated: async bus =>
                    {
                        await bus.Subscribe<ImporteraInkassoÄrendeJobbKommando>();
                        ////await bus.Subscribe<PingPongMessage>();
                    }
                );
            return services;
        }

        ////public static IServiceCollection AddMessageBus(
        ////    this IServiceCollection services,
        ////    IConfiguration configuration
        ////)
        ////{
        ////    return services
        ////        .AddHttpContextAccessor()
        ////        .AddSlimMessageBus(
        ////        builder =>
        ////        {
        ////            _ = builder
        ////                .AutoStartConsumersEnabled(true)
        ////                .AddChildBus(
        ////                    "Memory",
        ////                    (bus) =>
        ////                    {
        ////                        _ = bus.Produce<ImporteraInkassoÄrendeModell>(
        ////                                x => x.DefaultTopic(x.MessageType.Name)
        ////                            )
        ////                            .Consume<ImporteraInkassoÄrendeModell>(
        ////                                x =>
        ////                                    x.Topic(x.MessageType.Name)
        ////                                        .WithConsumer<ImporteraInkassoÄrendeMessageHandler>()
        ////                            )
        ////                            .WithProviderMemory(
        ////                                new MemoryMessageBusSettings
        ////                                {
        ////                                    EnableMessageSerialization = false
        ////                                }
        ////                            );
        ////                    }
        ////                )
        ////                .AddChildBus(
        ////                    "MainBus",
        ////                    bus =>
        ////                    {
        ////                        _ = bus.Produce<ImportAvInkassoÄrendeKölagt>(
        ////                                producer => producer.DefaultTopic(Topics.ÄrendeImport)
        ////                            )
        ////                            .Produce<PingPongMessage>(
        ////                                producer => producer.DefaultTopic(Topics.PingPong)
        ////                            )
        ////                            .WithProviderRedis(
        ////                                new RedisMessageBusSettings(
        ////                                    configuration.GetConnectionString("Redis")
        ////                                )
        ////                            );
        ////                    }
        ////                )
        ////                .WithProviderHybrid() // requires SlimMessageBus.Host.Hybrid package
        ////                .WithSerializer(new JsonMessageSerializer());
        ////        },
        ////        addConsumersFromAssembly: new[] { Assembly.GetExecutingAssembly() }
        ////    );
        ////}
    }
}
