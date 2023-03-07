using N3.CqrsEs.Ramverk;

namespace N3.CqrsEs.SkrivModell.Domän
{
    public interface IAggregateRepository
    {
        Task<T> LoadAsync<T>(string id, int? version = null, CancellationToken ct = default)
            where T : AbstraktAggregatRotBasKlass;

        Task StoreAsync<T>(T aggregate, bool newStream = false, CancellationToken ct = default)
            where T : AbstraktAggregatRotBasKlass;
    }
}
