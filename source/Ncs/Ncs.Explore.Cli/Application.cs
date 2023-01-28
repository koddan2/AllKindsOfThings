using Serilog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Ncs.Explore.Cli;

internal class Application
{
    public static async Task RunAsync()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        try
        {
            using var serviceProvider = CreateServices();
            using var scope = serviceProvider.CreateScope();

            var cfg = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var appConfig = scope.ServiceProvider.GetRequiredService<IOptions<AppConfig>>();

            await Testing.Produce1();
            Testing.Consume1();
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
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            var services = new ServiceCollection()
                .AddLogging(builder =>
                {
                    builder.AddSerilog(Log.Logger);
                })
                .AddSingleton<IConfiguration>(configuration)
                .Configure<AppConfig>(configuration)
                .BuildServiceProvider()
                ;

            return services;
        }
    }
}
