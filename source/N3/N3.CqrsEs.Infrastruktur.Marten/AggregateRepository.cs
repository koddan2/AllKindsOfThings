using Marten;
using N3.CqrsEs.Ramverk;

namespace N3.CqrsEs.SkrivModell.Domän
{
    public sealed class AggregateRepository
    {
        private readonly IDocumentStore _store;

        public AggregateRepository(IDocumentStore store)
        {
            _store = store;
        }

        public async Task StoreAsync<T>(T aggregate)
            where T:AbstraktAggregatBasKlass<T>
        {
            using (var session = _store.OpenSession())
            {
                // Take non-persisted events, push them to the event stream, indexed by the aggregate ID
                var events = aggregate.GetUncommittedEvents().ToArray();
                _ = session.Events.Append(aggregate.Id, aggregate.Version, events);
                await session.SaveChangesAsync();
            }
            // Once successfully persisted, clear events from list of uncommitted events
            aggregate.ClearUncommittedEvents();
        }

        public async Task<T> LoadAsync<T>(string id, int? version = null)
            where T : AbstraktAggregatBasKlass<T>
        {
            using (var session = _store.LightweightSession())
            {
                var aggregate = await session.Events.AggregateStreamAsync<T>(id, version ?? 0);
                return aggregate
                    ?? throw new InvalidOperationException($"No aggregate by id {id}.");
            }
        }
    }
}
