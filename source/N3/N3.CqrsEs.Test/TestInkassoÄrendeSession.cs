using CQRSlite.Domain;
using N3.CqrsEs.SkrivModell;

namespace N3.CqrsEs.Test
{
    internal class TestInkassoÄrendeSession : IInkassoÄrendeSession
    {
        private readonly ISession _session;

        public TestInkassoÄrendeSession(ISession session)
        {
            _session = session;
        }

        public async Task Add<T>(T aggregate, CancellationToken cancellationToken = default) where T : AggregateRoot
        {
            await _session.Add(aggregate, cancellationToken);
        }

        public async Task Commit(CancellationToken cancellationToken = default)
        {
            await _session.Commit(cancellationToken);
        }

        public async Task<T> Get<T>(Guid id, int? expectedVersion = null, CancellationToken cancellationToken = default) where T : AggregateRoot
        {
            return await _session.Get<T>(id, expectedVersion, cancellationToken);
        }
    }
}