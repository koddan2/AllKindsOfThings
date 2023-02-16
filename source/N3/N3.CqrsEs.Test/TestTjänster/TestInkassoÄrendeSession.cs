using Cqrs.Domain;
using N3.CqrsEs.SkrivModell;

namespace N3.CqrsEs.Test.TestTjänster
{
    internal class TestInkassoÄrendeSession : IInkassoÄrendeSession
    {
        private readonly IUnitOfWork<string> _session;

        public TestInkassoÄrendeSession(IUnitOfWork<string> session)
        {
            _session = session;
        }

        public void Add<TAggregateRoot>(TAggregateRoot aggregate, bool useSnapshots = false) where TAggregateRoot : IAggregateRoot<string>
        {
            throw new NotImplementedException();
        }

        public void Commit()
        {
            throw new NotImplementedException();
        }

        public TAggregateRoot Get<TAggregateRoot>(Guid id, int? expectedVersion = null, bool useSnapshots = false) where TAggregateRoot : IAggregateRoot<string>
        {
            throw new NotImplementedException();
        }

        public TAggregateRoot GetToDate<TAggregateRoot>(Guid id, DateTime versionedDate) where TAggregateRoot : IAggregateRoot<string>
        {
            throw new NotImplementedException();
        }

        public TAggregateRoot GetToVersion<TAggregateRoot>(Guid id, int version) where TAggregateRoot : IAggregateRoot<string>
        {
            throw new NotImplementedException();
        }
    }
}