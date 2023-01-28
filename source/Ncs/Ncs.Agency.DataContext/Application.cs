using Serilog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using FluentMigrator.Runner;
using Npgsql;
using SAK;

namespace Ncs.Agency.DataContext;

internal class Application
{
    public static void Run(string[] args)
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

            // Put the database update into a scope to ensure
            // that all resources will be disposed.
            if (args[0] == "up")
            {
                UpdateDatabase(scope.ServiceProvider);
            }
            else if (args[0] == "downto")
            {
                DowngradeDatabase(scope.ServiceProvider, long.Parse(args[1]));
            }
            else if (args[0] == "wipe")
            {
                DowngradeDatabase(scope.ServiceProvider, 0);
            }
            else if (args[0] == "dev-reset")
            {
                DowngradeDatabase(scope.ServiceProvider, 0);
                UpdateDatabase(scope.ServiceProvider);
            }
            else if (args[0] == "list")
            {
                ListMigrations(scope.ServiceProvider);
            }
            else
            {
                throw new ApplicationException($"Unknown command: {args[0]}");
            }
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

    private static void ListMigrations(IServiceProvider serviceProvider)
    {
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
        runner.ListMigrations();
    }

    /// <summary>
    /// Update the database
    /// </summary>
    private static void UpdateDatabase(IServiceProvider serviceProvider)
    {
        // Instantiate the runner
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

        // Execute the migrations
        runner.MigrateUp();
    }

    /// <summary>
    /// Update the database
    /// </summary>
    private static void DowngradeDatabase(IServiceProvider serviceProvider, long version)
    {
        // Instantiate the runner
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

        // Execute the migrations
        runner.MigrateDown(version);
    }
}
