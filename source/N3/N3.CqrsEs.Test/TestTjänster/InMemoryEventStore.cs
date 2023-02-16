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
                List<IEvent> list;
                if (_inMemoryDb.ContainsKey(@event.Id))
                {
                    list = _inMemoryDb[@event.Id];
                }
                else
                {
                    list = new List<IEvent>();
                    _inMemoryDb[@event.Id] = list;
                }

                list.Add(@event);
                await _publisher.Publish(@event, cancellationToken);
            }
        }

        public async Task<IEnumerable<IEvent>> Get(Guid aggregateId, int fromVersion, CancellationToken cancellationToken = default)
        {
            await ValueTask.CompletedTask;
            if (_inMemoryDb.TryGetValue(aggregateId, out List<IEvent>? value))
            {
                return value.Where(item => item.Version > fromVersion);
            }

            return Enumerable.Empty<IEvent>();
        }
    }
}
