using Serilog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ncs.Explore.Cli.KafkaTest;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System.Diagnostics;

namespace Ncs.Explore.Cli;

internal class Application
{
	public static async Task RunAsync()
	{
		await Task.CompletedTask;
		Log.Logger = new LoggerConfiguration()
			.WriteTo.Console()
			.CreateLogger();

		try
		{
			var serviceProvider = CreateServices();

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
