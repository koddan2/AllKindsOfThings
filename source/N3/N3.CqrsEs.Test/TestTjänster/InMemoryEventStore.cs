using Cqrs.Events;

namespace N3.CqrsEs.Test.TestTjänster
{
    public class InMemoryEventStore : IEventStore<string>, IEventPublisher<string>
    {
        private readonly Dictionary<Guid, List<IEvent<string>>> _inMemoryDb = new();

        public InMemoryEventStore()
        {
        }

        IEnumerable<IEvent<string>> IEventStore<string>.Get<T>(Guid aggregateId, bool useLastEventOnly, int fromVersion)
        {
            throw new NotImplementedException();
        }

        IEnumerable<IEvent<string>> IEventStore<string>.Get(Type aggregateRootType, Guid aggregateId, bool useLastEventOnly, int fromVersion)
        {
            throw new NotImplementedException();
        }

        IEnumerable<EventData> IEventStore<string>.Get(Guid correlationId)
        {
            throw new NotImplementedException();
        }

        IEnumerable<IEvent<string>> IEventStore<string>.GetBetweenDates(Type aggregateRootType, Guid aggregateId, DateTime fromVersionedDate, DateTime toVersionedDate)
        {
            throw new NotImplementedException();
        }

        IEnumerable<IEvent<string>> IEventStore<string>.GetBetweenDates<T>(Guid aggregateId, DateTime fromVersionedDate, DateTime toVersionedDate)
        {
            throw new NotImplementedException();
        }

        IEnumerable<IEvent<string>> IEventStore<string>.GetToDate(Type aggregateRootType, Guid aggregateId, DateTime versionedDate)
        {
            throw new NotImplementedException();
        }

        IEnumerable<IEvent<string>> IEventStore<string>.GetToDate<T>(Guid aggregateId, DateTime versionedDate)
        {
            throw new NotImplementedException();
        }

        IEnumerable<IEvent<string>> IEventStore<string>.GetToVersion(Type aggregateRootType, Guid aggregateId, int version)
        {
            throw new NotImplementedException();
        }

        IEnumerable<IEvent<string>> IEventStore<string>.GetToVersion<T>(Guid aggregateId, int version)
        {
            throw new NotImplementedException();
        }

        void IEventPublisher<string>.Publish<TEvent>(TEvent @event)
        {
            throw new NotImplementedException();
        }

        void IEventPublisher<string>.Publish<TEvent>(IEnumerable<TEvent> events)
        {
            throw new NotImplementedException();
        }

        void IEventStore<string>.Save<T>(IEvent<string> @event)
        {
            throw new NotImplementedException();
        }

        void IEventStore<string>.Save(Type aggregateRootType, IEvent<string> @event)
        {
            throw new NotImplementedException();
        }
    }
}
