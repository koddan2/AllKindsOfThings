using EventStore.Client;
using Ncs.Domain.Model;
using Serilog;
using System.Text.Json;
using SAK;
using System.Runtime.CompilerServices;

namespace Ncs.EventSourcing
{
	public class EventStoreDbMediator : IEventStore
	{
		private readonly ILogger _log;
		private readonly EventStoreClient _eventStoreClient;

		public EventStoreDbMediator(ILogger log, EventStoreClient eventStoreClient)
		{
			_log = log;
			_eventStoreClient = eventStoreClient;
		}

		async IAsyncEnumerable<EventRecord> IEventStore.ReadCategoryStreamFullAsync(string aggregateName, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			var readResult = _eventStoreClient.ReadStreamAsync(
				Direction.Forwards,
				new AggregateStreamCategory(aggregateName).FormatStreamName(),
				StreamPosition.Start,
				////resolveLinkTos: true,
				cancellationToken: cancellationToken);

			await foreach (var @event in readResult)
			{
				yield return @event.Event;
			}
		}

		async IAsyncEnumerable<EventRecord> IEventStore.ReadStreamFullAsync(string aggregateName, string id, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			var readResult = _eventStoreClient.ReadStreamAsync(
				Direction.Forwards,
				new AggregateStream(DebtCollectionClientAggregate.AggregateName, id).FormatStreamName(),
				StreamPosition.Start,
				cancellationToken: cancellationToken);

			await foreach (var @event in readResult)
			{
				yield return @event.Event;
			}
		}

		async Task<IWriteResult> IEventStore.StoreEventAsync(IDomainEvent @event, CancellationToken cancellationToken)
		{
			var eventData = new EventData(
				Uuid.NewUuid(),
				@event.EventName,
				JsonSerializer.SerializeToUtf8Bytes((object)@event)
			);

			////{
			////	var dat = JsonSerializer.Deserialize<DebtCollectionClientCreatedEvent_V1>(eventData.Data.ToArray());
			////	var a = dat;
			////}

			var result = await _eventStoreClient.AppendToStreamAsync(
				@event.FormatStreamName(),
				StreamState.Any,
				new[] { eventData },
				cancellationToken: cancellationToken);

			_log.Information("Stored event: {result}", result.ToJson());
			return result;
		}
	}
}