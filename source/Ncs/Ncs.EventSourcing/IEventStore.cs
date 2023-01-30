using EventStore.Client;
using Ncs.Domain.Model;

namespace Ncs.EventSourcing
{
	public interface IEventStore
	{
		Task<IWriteResult> StoreEventAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);

		IAsyncEnumerable<EventRecord> ReadStreamFullAsync(string aggregateName, string id, CancellationToken cancellationToken = default);
	}
}