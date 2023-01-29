using Serilog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Ncs.Explore.Cli.KafkaTest;
using System.Text.Json;

namespace Ncs.Explore.Cli;

internal static class ExploreExtensions
{
    private static readonly JsonSerializerOptions _JsonSerializerOptions = new() { WriteIndented = true };
    public static string ToJson<T>(this T instance)
    {
        return JsonSerializer.Serialize(instance, _JsonSerializerOptions);
    }

    public static IEnumerable<KeyValuePair<string, string?>> GetSubSectionOnly(this IConfiguration configuration, string sectionName)
    {
        return configuration
            .GetSection(sectionName)
            .AsEnumerable()
            .ToDictionary(x => x.Key.Replace($"{sectionName}:", ""), y => y.Value)
            .AsEnumerable();
    }
}

internal class Application
{
    public static void Run()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        try
        {

            var t1 = new Thread(() =>
            {
                try
                {
                    using var serviceProvider = CreateServices();
                    using var scope = serviceProvider.CreateScope();
                    var cfg = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                    var appConfig = scope.ServiceProvider.GetRequiredService<IOptions<AppConfig>>();

                    var consumer = scope.ServiceProvider.GetRequiredService<KafkaTestConsumer1>();
                    consumer.Run(cfg.GetSubSectionOnly("KafkaTest.1").Where(kvp => kvp.Value is not null));
                }
                catch (Exception e)
                {
                    Log.Error(e, "Unexpected!");
                    throw;
                }
            });
            var t2 = new Thread(() =>
            {
                try
                {
                    using var serviceProvider = CreateServices();
                    using var scope = serviceProvider.CreateScope();
                    var cfg = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                    var appConfig = scope.ServiceProvider.GetRequiredService<IOptions<AppConfig>>();

                    var producer = scope.ServiceProvider.GetRequiredService<KafkaTestProducer1>();
                    producer.Run(cfg.GetSubSectionOnly("KafkaTest.1").Where(kvp => kvp.Value is not null));
                }
                catch (Exception e)
                {
                    Log.Error(e, "Unexpected!");
                    throw;
                }
            });

            t1.Start();
            t2.Start();

            t1.Join();
            t2.Join();

            ////try
            ////{
            ////    using var serviceProvider = CreateServices();
            ////    using var scope = serviceProvider.CreateScope();
            ////    var cfg = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            ////    var appConfig = scope.ServiceProvider.GetRequiredService<IOptions<AppConfig>>();

            ////    var consumer = scope.ServiceProvider.GetRequiredService<KafkaTestConsumer1>();
            ////    consumer.Run(cfg.GetSubSectionOnly("KafkaTest.3").Where(kvp => kvp.Value is not null));
            ////}
            ////catch (Exception e)
            ////{
            ////    Log.Error(e, "Unexpected!");
            ////    throw;
            ////}
        }
        catch (Exception e)
        {
            Log.Logger.Fatal(e, "Unexpected!");
            throw;
        }
        finally
        {
            Log.CloseAndFlush();
        }

        static ServiceProvider CreateServices()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true)
                .AddIniFile("appsettings.ini", true)
                .Build()
                ;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            var services = new ServiceCollection()
                .AddLogging(builder =>
                {
                    builder.AddSerilog(Log.Logger);
                })
                .AddTransient<ILogger>((_) => Log.Logger)
                .AddSingleton<IConfiguration>(configuration)
                .AddTransient<KafkaTestConsumer1>()
                .AddTransient<KafkaTestProducer1>()
                .Configure<AppConfig>(configuration)
                .BuildServiceProvider()
                ;

            return services;
        }
    }
}
