using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using SqlKata.Compilers;
using SqlKata.Execution;

var rootLogger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();
Log.Logger = rootLogger;

try
{

    var host = Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration(ConfigureAppConfiguration)
        .ConfigureLogging(ConfigureLogging)
        .ConfigureServices(ConfigureServices)
        .Build();

    var dbMigrator = host.Services.GetRequiredService<DatabaseMigrator>();
    var app = host.Services.GetRequiredService<Application>();

    dbMigrator.Migrate();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unexpected error.");
}
finally
{
    Log.Information("Bye.");
    Log.CloseAndFlush();
}

void ConfigureLogging(HostBuilderContext hostBuilder, ILoggingBuilder loggingBuilder)
{
    _ = loggingBuilder
        .AddSerilog();
}

void ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection services)
{
    _ = services
        .AddSingleton<SqliteConnection>((context) =>
        {
            var connString = new SqliteConnectionStringBuilder
            {
                DataSource = "application.db",
                Mode = SqliteOpenMode.ReadWriteCreate,
                ForeignKeys = true,
            };

            var conn = new SqliteConnection(connString.ToString());
            conn.Open();
            return conn;
        })
        .AddSingleton<QueryFactory>((context) =>
        {
            var conn = context.GetRequiredService<SqliteConnection>();
            return new QueryFactory(conn, new SqlServerCompiler());
        })
        .AddTransient<DatabaseMigrator>()
        .AddTransient<Application>();
}

void ConfigureAppConfiguration(HostBuilderContext hostBuilderContext, IConfigurationBuilder configBuilder)
{
    _ = configBuilder
        .AddCommandLine(args)
        .AddEnvironmentVariables()
        .AddUserSecrets(System.Reflection.Assembly.GetExecutingAssembly())
        ;
};