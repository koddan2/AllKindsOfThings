using N3.CqrsEs.Messages;
using SlimMessageBus.Host.Redis;
using N3.CqrsEs;
using N3.App.Ombud.Web.MessageHandlers;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using LanguageExt;
using Microsoft.AspNetCore.Components;

namespace N3.App.Ombud.Web
{
    public static class SetupServices
    {
        public static void AddBasicServices(
            this IServiceCollection services,
            IConfiguration configuration,
            IHostEnvironment hostEnvironment
        )
        {
            Console.WriteLine(hostEnvironment.EnvironmentName);
            _ = services.AddHttpContextAccessor();
            _ = services.AddMessageBus0(configuration);
        }

        public static IServiceCollection AddMessageBus0(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            _ = services
                .AddRebusHandler<ImportAvInkassoÄrendeKölagtMessageHandler>()
                .AddRebusHandler<PingPongMessageHandler>()
                .AddRebus(
                    builder =>
                    {
                        return builder
                            .Transport(
                                transport =>
                                    transport.UsePostgreSql(
                                        configuration.GetConnectionString("RebusPostgres"),
                                        "main_bus",
                                        Channels.N3DomainCommon
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
                                        .MapAssemblyOf<CqrsEsAssemblyMarker>(
                                            Channels.N3DomainCommon
                                        )
                            );
                    },
                    onCreated: async bus =>
                    {
                        await bus.Subscribe<ImportAvInkassoÄrendeKölagt>();
                        await bus.Subscribe<PingPongMessage>();
                    }
                );

            return services;
        }

        ////public static IServiceCollection AddMessageBus0(
        ////    this IServiceCollection services,
        ////    IConfiguration configuration
        ////)
        ////{
        ////    var redisConnString = configuration.GetConnectionString("Redis");
        ////    ////_ = SlimMessageBus.Host.MsDependencyInjection.ServiceCollectionExtensions.AddMessageBusConsumersFromAssembly(services, Assembly.GetExecutingAssembly());
        ////    return services
        ////        .AddHttpContextAccessor()
        ////        ////.AddMessageBusAspNet()
        ////        .AddHostedService<BootstrapperBackgroundService>()
        ////        ////.AddTransient<ImporteraInkassoÄrendeMessageHandler>()
        ////        ////.AddTransient<ImportAvInkassoÄrendeKölagtMessageHandler>()
        ////        .AddSlimMessageBus(
        ////            builder =>
        ////            {
        ////                _ = builder
        ////                    .AutoStartConsumersEnabled(true)
        ////                    .Consume<ImportAvInkassoÄrendeKölagt>(
        ////                        consumer =>
        ////                            consumer
        ////                                .WithConsumer<ImportAvInkassoÄrendeKölagtMessageHandler>()
        ////                                .Instances(1)
        ////                                .Topic(Topics.ÄrendeImport)
        ////                    )
        ////                    .Consume<PingPongMessage>(
        ////                        consumer =>
        ////                            consumer
        ////                                .WithConsumer<PingPongMessageHandler>()
        ////                                .Instances(1)
        ////                                .Topic(Topics.PingPong)
        ////                    ////.AttachEvents(ev =>
        ////                    ////{
        ////                    ////    ev.OnMessageArrived = (a, b, c, d, e) =>
        ////                    ////    {
        ////                    ////        Console.WriteLine("asdf");
        ////                    ////    };
        ////                    ////})
        ////                    )
        ////                    .WithProviderRedis(new RedisMessageBusSettings(redisConnString))
        ////                    .WithSerializer(new JsonMessageSerializer())
        ////                .AttachEvents(events =>
        ////                {
        ////                    // Invoke the action for the specified message type when sent via the bus:
        ////                    events.OnMessageArrived = (
        ////                        bus,
        ////                        consumerSettings,
        ////                        message,
        ////                        path,
        ////                        nativeMessage
        ////                    ) =>
        ////                    {
        ////                        Console.WriteLine(
        ////                            "The message: {0} arrived on the topic/queue {1}",
        ////                            message,
        ////                            path
        ////                        );
        ////                    };
        ////                    events.OnMessageFault = (
        ////                        bus,
        ////                        consumerSettings,
        ////                        message,
        ////                        ex,
        ////                        nativeMessage
        ////                    ) =>
        ////                    { };
        ////                    events.OnMessageFinished = (
        ////                        bus,
        ////                        consumerSettings,
        ////                        message,
        ////                        path,
        ////                        nativeMessage
        ////                    ) =>
        ////                    {
        ////                        Console.WriteLine(
        ////                            "The SomeMessage: {0} finished on the topic/queue {1}",
        ////                            message,
        ////                            path
        ////                        );
        ////                    };
        ////                });
        ////                ;
        ////            },
        ////            addConsumersFromAssembly: new[] { typeof(Program).Assembly }
        ////        );
        ////}
    }
}
