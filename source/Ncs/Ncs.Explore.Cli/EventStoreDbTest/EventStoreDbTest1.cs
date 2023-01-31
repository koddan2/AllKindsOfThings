using EventStore.Client;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ncs.Explore.Cli.EventStoreDbTest
{
	internal class EventStoreDbTest1
	{
		public async Task Test1(string esdbConnString)
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
					"$ce-debt",
					StreamPosition.Start,
					cancellationToken: cts.Token);
				var events2 = await result2.ToListAsync(cts.Token);
				Log.Information("JSON: {json}", events2.ToJson());
				foreach (var @event in events2)
				{
					Log.Information(Encoding.UTF8.GetString(@event.Event.Data.ToArray()));
				}
			}
		}

		public async Task Test2()
		{
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
		}
	}
}
