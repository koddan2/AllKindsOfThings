using SlimMessageBus.Host.AspNetCore;
using SlimMessageBus.Host.Serialization.SystemTextJson;
using N3.CqrsEs.Messages;
using SlimMessageBus.Host.Redis;
using N3.CqrsEs;
using N3.App.Ombud.Web.MessageHandlers;
using System.Reflection;

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
            _ = services.AddMessageBus(configuration);
        }

        public static IServiceCollection AddMessageBus(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            var redisConnString = configuration.GetConnectionString("Redis");
            ////_ = SlimMessageBus.Host.MsDependencyInjection.ServiceCollectionExtensions.AddMessageBusConsumersFromAssembly(services, Assembly.GetExecutingAssembly());
            return services
                .AddHttpContextAccessor()
                .AddHostedService<BootstrapperBackgroundService>()
                ////.AddTransient<ImporteraInkassoÄrendeMessageHandler>()
                ////.AddTransient<ImportAvInkassoÄrendeKölagtMessageHandler>()
                .AddSlimMessageBus(
                    builder =>
                    {
                        _ = builder
                            .AutoStartConsumersEnabled(true)
                            //.Consume<PingPongMessage>(
                            //    consumer =>
                            //        consumer
                            //            .WithConsumer<PingPongMessageHandler>()
                            //            .Instances(1)
                            //            .Topic(Topics.PingPong)
                            //)
                            .Produce<ImportAvInkassoÄrendeKölagt>(p => p.DefaultTopic(Topics.ÄrendeImport))
                            .Consume<ImportAvInkassoÄrendeKölagt>(
                                consumer =>
                                    consumer
                                        .WithConsumer<ImportAvInkassoÄrendeKölagtMessageHandler>()
                                        .Instances(1)
                                        .Topic(Topics.ÄrendeImport)
                            ////.AttachEvents(ev =>
                            ////{
                            ////    ev.OnMessageArrived = (a, b, c, d, e) =>
                            ////    {
                            ////        Console.WriteLine("asdf");
                            ////    };
                            ////})
                            )
                            .WithProviderRedis(new RedisMessageBusSettings(redisConnString))
                            .WithSerializer(new JsonMessageSerializer())
                        ////.AttachEvents(events =>
                        ////{
                        ////    // Invoke the action for the specified message type when sent via the bus:
                        ////    events.OnMessageArrived = (
                        ////        bus,
                        ////        consumerSettings,
                        ////        message,
                        ////        path,
                        ////        nativeMessage
                        ////    ) =>
                        ////    {
                        ////        Console.WriteLine(
                        ////            "The message: {0} arrived on the topic/queue {1}",
                        ////            message,
                        ////            path
                        ////        );
                        ////    };
                        ////    events.OnMessageFault = (
                        ////        bus,
                        ////        consumerSettings,
                        ////        message,
                        ////        ex,
                        ////        nativeMessage
                        ////    ) => { };
                        ////    events.OnMessageFinished = (
                        ////        bus,
                        ////        consumerSettings,
                        ////        message,
                        ////        path,
                        ////        nativeMessage
                        ////    ) =>
                        ////    {
                        ////        Console.WriteLine(
                        ////            "The SomeMessage: {0} finished on the topic/queue {1}",
                        ////            message,
                        ////            path
                        ////        );
                        ////    };
                        ////});
                        ;
                    },
                    addConsumersFromAssembly: new[] { typeof(Program).Assembly }
                );
        }
    }
}
