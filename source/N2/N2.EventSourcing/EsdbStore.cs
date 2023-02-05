using EventStore.Client;
using Microsoft.Extensions.Options;
using N2.Domain;
using N2.EventSourcing.Common;
using System.Text;
using System.Text.Json;

namespace N2.EventSourcing
{
	public class EsdbStore : IEventSender, IEventReader
	{
		private static readonly JsonSerializerOptions _JsonSerializerOptions = new() { WriteIndented = true };

		private readonly EventStoreClient _client;
		private readonly N2Registry _registry;

		public EsdbStore(IOptions<N2EventSourcingConfiguration> options, N2Registry registry)
		{
			var cfg = options.Value;
			var settings = EventStoreClientSettings
				.Create(cfg.EventStoreDbConnectionString);
			_client = new EventStoreClient(settings);
			_registry = registry;
		}

		async Task<IEnumerable<EventReadResult>> IEventReader.ReadFrom(string streamName, ulong position)
		{
			var readStreamResult = _client.ReadStreamAsync(
				Direction.Forwards,
				streamName,
				StreamPosition.FromInt64((long)position));

			var resolvedEvents = await readStreamResult.ToListAsync();

			return resolvedEvents.Select(Deserialize);
		}

		IAsyncEnumerable<EventReadResult> IEventReader.ReadAllEvents(string eventType, ulong position, ulong count)
		{
			var eventMetadata = new EventMetadata(eventType);
			var readStreamResult = _client.ReadStreamAsync(
				Direction.Forwards,
				eventMetadata.GetStreamNameForEvent(),
				StreamPosition.FromInt64((long)position),
				maxCount: (long)count);

			return readStreamResult.Select(Deserialize)
#if DEBUG
				.Where(x =>
				{
					if (eventMetadata.Validate(x.Event) is false)
					{
						throw new InvalidOperationException("The name does not ");
					}
					return true;
				})
#endif
				;
		}

		async Task<IEnumerable<EventReadResult>> IEventReader.Read(string streamName, ExpectedStateOfStream expectedState)
		{
			var readStreamResult = _client.ReadStreamAsync(
				Direction.Forwards,
				streamName,
				StreamPosition.Start);

			var readState = await readStreamResult.ReadState;
			switch (expectedState)
			{
				case ExpectedStateOfStream.Absent:
					if (readState != ReadState.StreamNotFound)
					{
						throw new ExpectationFailedException($"Expectation was: {expectedState} but stream exists.");
					}
					else
					{
						break;
					}
				case ExpectedStateOfStream.Exist:
					if (readState != ReadState.Ok)
					{
						throw new ExpectationFailedException($"Expectation was: {expectedState} but stream does not exist.");
					}
					else
					{
						break;
					}
			}

			var resolvedEvents = await readStreamResult.ToListAsync();

			return resolvedEvents.Select(Deserialize);
		}

		private EventReadResult Deserialize(ResolvedEvent resolved)
		{
			var data = resolved.Event.Data;
			var json = Encoding.UTF8.GetString(data.ToArray());
			var name = resolved.Event.EventType;
			var type = _registry.Events[name];
			var @object = JsonSerializer.Deserialize(json, type);
			var eventObject = @object as IEvent
				?? throw new InvalidDataException($"Could not deserialize {json} into a type that is an {nameof(IEvent)}");
			var result = new EventReadResult(eventObject, resolved.Event.EventNumber);
			return result;
		}

		async Task<ulong> IEventSender.Send(string streamName, IEvent @event)
		{
			var json = JsonSerializer.Serialize((object)@event, _JsonSerializerOptions);
			var body = Encoding.UTF8.GetBytes(json);
			var eventData = new EventData(Uuid.NewUuid(), streamName, body);

			var writeResult = await _client.AppendToStreamAsync(
				streamName,
				StreamState.Any,
				new[] { eventData });

			return writeResult.NextExpectedStreamRevision.ToUInt64();
		}
	}
}