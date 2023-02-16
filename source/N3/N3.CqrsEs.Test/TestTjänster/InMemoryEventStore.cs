using Cqrs.Events;

namespace N3.CqrsEs.Test.TestTjänster
{
    public class InMemoryEventStore : IEventStore<string>
    {
        private readonly IEventPublisher<string> _publisher;
        private readonly Dictionary<Guid, List<IEvent<string>>> _inMemoryDb = new();

        public InMemoryEventStore(IEventPublisher<string> publisher)
        {
            _publisher = publisher;
        }

        public async Task Save(IEnumerable<IEvent<string>> events, CancellationToken cancellationToken = default)
        {
            await ValueTask.CompletedTask;
            foreach (var @event in events)
            {
                List<IEvent<string>> list;
                if (_inMemoryDb.ContainsKey(@event.Id))
                {
                    list = _inMemoryDb[@event.Id];
                }
                else
                {
                    list = new List<IEvent<string>>();
                    _inMemoryDb[@event.Id] = list;
                }

                list.Add(@event);
                _publisher.Publish(@event);
            }
        }

        public async Task<IEnumerable<IEvent<string>>> Get(Guid aggregateId, int fromVersion, CancellationToken cancellationToken = default)
        {
            await ValueTask.CompletedTask;
            if (_inMemoryDb.TryGetValue(aggregateId, out List<IEvent<string>>? value))
            {
                return value.Where(item => item.Version > fromVersion);
            }

            return Enumerable.Empty<IEvent<string>>();
        }

        public void Save<T>(IEvent<string> @event)
        {
            throw new NotImplementedException();
        }

        public void Save(Type aggregateRootType, IEvent<string> @event)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IEvent<string>> Get<T>(Guid aggregateId, bool useLastEventOnly = false, int fromVersion = -1)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IEvent<string>> Get(Type aggregateRootType, Guid aggregateId, bool useLastEventOnly = false, int fromVersion = -1)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IEvent<string>> GetToVersion(Type aggregateRootType, Guid aggregateId, int version)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IEvent<string>> GetToVersion<T>(Guid aggregateId, int version)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IEvent<string>> GetToDate(Type aggregateRootType, Guid aggregateId, DateTime versionedDate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IEvent<string>> GetToDate<T>(Guid aggregateId, DateTime versionedDate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IEvent<string>> GetBetweenDates(Type aggregateRootType, Guid aggregateId, DateTime fromVersionedDate, DateTime toVersionedDate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IEvent<string>> GetBetweenDates<T>(Guid aggregateId, DateTime fromVersionedDate, DateTime toVersionedDate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EventData> Get(Guid correlationId)
        {
            throw new NotImplementedException();
        }
    }
}
