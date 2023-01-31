using EventStore.Client;
using Ncs.Domain.Model;
using SAK;
using Serilog;

namespace Ncs.EventSourcing
{
	public class DebtCollectionClientCommandHandler // : ICommandHandler
	{
		private readonly ILogger _log;
		private readonly IEventStore _eventStore;

		public DebtCollectionClientCommandHandler(ILogger log, IEventStore eventStore)
		{
			_log = log;
			_eventStore = eventStore;
		}

		public async Task Handle(DebtCollectionClientEntity entity, CreateDebtCollectionCommand command)
		{
			DebtCollectionClientCreatedEvent_V1 @event = new(entity.Id, command.PersonalIdentificationNumber, command.Name);
			var result = await _eventStore.StoreEventAsync(@event);
			_log.Information("Result: {result}", result.ToJson());
		}
	}
	////public interface ICommandHandler
	////{
	////	Task Handle(IAggregate entity, ICommand command);
	////}
	public interface IEventStore
	{
		Task<IWriteResult> StoreEventAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);

		IAsyncEnumerable<EventRecord> ReadStreamFullAsync(string aggregateName, string id, CancellationToken cancellationToken = default);

		IAsyncEnumerable<EventRecord> ReadCategoryStreamFullAsync(string aggregateName, CancellationToken cancellationToken = default);
	}
}