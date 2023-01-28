using Serilog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using FluentMigrator.Runner;
using Npgsql;
using SAK;

namespace Ncs.Explore.Cli;

internal class Application
{
    public static void Run()
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

        var preludeAppConfig = configuration.Get<AppConfig>().OrFail();

        var services = new ServiceCollection()
            .AddLogging(builder =>
            {
                _ = builder.AddSerilog(Log.Logger);
            })
            .AddSingleton<IConfiguration>(configuration)
            .Configure<AppConfig>(configuration)
            // Add common FluentMigrator services
            .AddFluentMigratorCore()
            .ConfigureRunner(builder => builder
                // Add DB support to FluentMigrator
                .AddPostgres11_0()
                .WithGlobalConnectionString(GetNpgsqlConnectionString(preludeAppConfig))
                // Define the assembly containing the migrations
                .ScanIn(typeof(Application).Assembly).For.Migrations())
            // Enable logging to console in the FluentMigrator way
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            // Build the service provider
            .BuildServiceProvider(false)
            ;

        return services;
    }

    private static string GetNpgsqlConnectionString(AppConfig appConfig)
    {
        var builder = new NpgsqlConnectionStringBuilder();
        var pgConfig = appConfig.Postgres.OrFail();
        builder.Host = pgConfig.Host;
        builder.Database = pgConfig.Database;
        builder.Username = pgConfig.Username;
        builder.Password = pgConfig.Password;
        return builder.ToString();
    }
}
