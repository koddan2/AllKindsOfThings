using Marten;
using N3.CqrsEs.Ramverk;

namespace N3.CqrsEs.SkrivModell.Domän
{
    public sealed class AggregateRepository : IAggregateRepository
    {
        private readonly IDocumentStore _store;

        public AggregateRepository(IDocumentStore store)
        {
            this._store = store;
        }

        public async Task StoreAsync<T>(T aggregate, CancellationToken ct = default)
            where T : AbstraktAggregatBasKlass
        {
            await using var session = _store.LightweightSession();
            // Take non-persisted events, push them to the event stream, indexed by the aggregate ID
            var events = aggregate.GetUncommittedEvents().ToArray();
            if (aggregate.Version == events.Length)
            {
                _ = session.Events.StartStream<T>(aggregate.Id, events);
            }
            else
            {
                _ = session.Events.Append(aggregate.Id, aggregate.Version, events);
            }
            await session.SaveChangesAsync(ct);
            // Once successfully persisted, clear events from list of uncommitted events
            aggregate.ClearUncommittedEvents();
        }

        public async Task<T> LoadAsync<T>(
            string id,
            int? version = null,
            CancellationToken ct = default
        ) where T : AbstraktAggregatBasKlass
        {
            await using var session = _store.LightweightSession();
            var aggregate = await session.Events.AggregateStreamAsync<T>(id, version ?? 0, token: ct);
            return aggregate ?? throw new InvalidOperationException($"No aggregate by id {id}.");
        }
    }
}
