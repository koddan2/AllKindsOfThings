using CQRSlite.Events;

namespace N3.CqrsEs.Test.TestTjänster
{
    public class InMemoryEventStore : IEventStore
    {
        private readonly IEventPublisher _publisher;
        private readonly Dictionary<Guid, List<IEvent>> _inMemoryDb = new();

        public InMemoryEventStore(IEventPublisher publisher)
        {
            _publisher = publisher;
        }

        public async Task Save(IEnumerable<IEvent> events, CancellationToken cancellationToken = default)
        {
            foreach (var @event in events)
            {
                var found = _inMemoryDb.TryGetValue(@event.Id, out var list);
                if (!found || list is null)
                {
                    list = new List<IEvent>();
                    _inMemoryDb.Add(@event.Id, list);
                }
                list.Add(@event);
                await _publisher.Publish(@event, cancellationToken);
            }
        }

        public async Task<IEnumerable<IEvent>> Get(Guid aggregateId, int fromVersion, CancellationToken cancellationToken = default)
        {
            await ValueTask.CompletedTask;
            var found = _inMemoryDb.TryGetValue(aggregateId, out var events);
            return found && events is not null
                ? events.Where(x => x.Version > fromVersion)
                : Array.Empty<IEvent>();
        }
    }
}
