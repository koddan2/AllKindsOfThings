using CQRSlite.Events;

namespace N3.CqrsEs.Test.Malpåse
{
    internal class TestEventStore : IEventStore
    {
        private readonly Dictionary<Guid, List<IEvent>> _lagring = new();
        public async Task<IEnumerable<IEvent>> Get(Guid aggregateId, int fromVersion, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            if (!_lagring.ContainsKey(aggregateId))
            {
                _lagring[aggregateId] = new List<IEvent>();
            }
            return _lagring[aggregateId].Skip(fromVersion);
        }

        public async Task Save(IEnumerable<IEvent> events, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            foreach (var item in events)
            {
                if (!_lagring.ContainsKey(item.Id))
                {
                    _lagring[item.Id] = new List<IEvent>();
                }
                _lagring[item.Id].Add(item);
            }
        }
    }
}