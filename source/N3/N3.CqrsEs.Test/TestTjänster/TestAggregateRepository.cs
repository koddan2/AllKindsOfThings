using N3.CqrsEs.Ramverk;
using N3.CqrsEs.SkrivModell.Domän;
using System.Text.Json;

namespace N3.CqrsEs.Test.TestTjänster
{
    internal class TestAggregateRepository : IAggregateRepository
    {
        private readonly IHändelseKassa _händelseKassa;

        public TestAggregateRepository(IHändelseKassa händelseKassa)
        {
            _händelseKassa = händelseKassa;
        }

        public async Task<T> LoadAsync<T>(
            string id,
            int? version = null,
            CancellationToken ct = default
        )
            where T : AbstraktAggregatBasKlass
        {
            var result = JsonSerializer.Deserialize<T>("{}");
            var ev = await _händelseKassa.Hämta(new AggregatStrömIdentifierare<T>(id));
            if (result is InkassoÄrende inkassoÄrende)
            {
                foreach (var e in ev)
                {
                    inkassoÄrende._Applicera(e);
                }
            }
            return result!;
        }

        public async Task StoreAsync<T>(T aggregate, CancellationToken ct = default)
            where T : AbstraktAggregatBasKlass
        {
            foreach (IHändelse item in aggregate.GetUncommittedEvents())
            {
                await _händelseKassa.Registrera(
                    new AggregatStrömIdentifierare<T>(aggregate.Id),
                    item,
                    HändelseModus.SkapaNy
                );
            }
        }
    }
}
