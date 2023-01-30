using Serilog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Ncs.Explore.Cli.KafkaTest;
using System.Text.Json;
using EventStore.Client;
using System.Threading;
using System.Text;

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
	public static async Task RunAsync()
	{
		Log.Logger = new LoggerConfiguration()
			.WriteTo.Console()
			.CreateLogger();


		////var esdbConnString = "esdb://localhost:2111?tls=true&tlsVerifyCert=false";
		var esdbConnString = "esdb://localhost:2113?tls=false";
		try
		{
			var settings = EventStoreClientSettings.Create(esdbConnString);
			settings.ConnectionName = "Projection management client";
			settings.DefaultCredentials = new UserCredentials("admin", "changeit");
			var managementClient = new EventStoreProjectionManagementClient(settings);
			await foreach (var proj in managementClient.ListAllAsync())
			{
				Log.Information(proj.Name);
			}
			var a = await managementClient.GetResultAsync("$by_category");
			Log.Information("{a}", a.ToJson());
		}
		catch (Exception)
		{

			throw;
		}

		try
		{
			var cts = new CancellationTokenSource();
			cts.CancelAfter(TimeSpan.FromSeconds(10));

			var settings = EventStoreClientSettings
				.Create(esdbConnString);
			var client = new EventStoreClient(settings);

			var evt = new TestEvent(
				Guid.NewGuid().ToString("N"),
				ImportantData: "I wrote my first event!"
			);

			var eventData = new EventData(
				Uuid.NewUuid(),
				"TestEvent",
				JsonSerializer.SerializeToUtf8Bytes(evt)
			);
			await client.AppendToStreamAsync(
				"some-stream",
				StreamState.Any,
						new[] { eventData },
				cancellationToken: cts.Token
			);

			var result = client.ReadStreamAsync(
				Direction.Forwards,
				"some-stream",
				StreamPosition.Start,
				cancellationToken: cts.Token);

			var events = await result.ToListAsync(cts.Token);
			foreach (var @event in events)
			{
				//Log.Information(Encoding.UTF8.GetString(@event.Event.Data.ToArray()));
			}

			//Log.Information("JSON: {json}", events.ToJson());

			{
				var result2 = client.ReadStreamAsync(
					Direction.Forwards,
					//"$by_category",
					"$streams",
					StreamPosition.Start,
					cancellationToken: cts.Token);
				var events2 = await result.ToListAsync(cts.Token);
				Log.Information("JSON: {json}", events2.ToJson());
				foreach (var @event in events2)
				{
					Log.Information(Encoding.UTF8.GetString(@event.Event.Data.ToArray()));
				}
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

	public static void RunKafkaTest1()
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
	}

	public static void RunKafkaTest2()
	{
		try
		{
			using var serviceProvider = CreateServices();
			using var scope = serviceProvider.CreateScope();
			var cfg = scope.ServiceProvider.GetRequiredService<IConfiguration>();
			var appConfig = scope.ServiceProvider.GetRequiredService<IOptions<AppConfig>>();

			var consumer = scope.ServiceProvider.GetRequiredService<KafkaTestConsumer1>();
			consumer.Run(cfg.GetSubSectionOnly("KafkaTest.3").Where(kvp => kvp.Value is not null));
		}
		catch (Exception e)
		{
			Log.Error(e, "Unexpected!");
			throw;
		}
	}
}
