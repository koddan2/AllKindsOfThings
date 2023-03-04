using Marten;
using Microsoft.Win32;
using N3.CqrsEs.Ramverk;

namespace N3.CqrsEs.Infrastruktur.Marten
{
    public class MartenHändelseKassa : IHändelseKassa
    {
        private readonly IDocumentStore _store;

        public MartenHändelseKassa(IDocumentStore store)
        {
            _store = store;
        }

        public async Task<IEnumerable<IHändelse>> Hämta<T>(AggregatStrömIdentifierare<T> ström)
            where T : IAggregatRot
        {
            await using var session = _store.LightweightSession();
            var events = await session.Events.FetchStreamAsync(ström.ByggStrömIdentifierare());
            return events.Select(x => (IHändelse)x.Data);
        }

        public async Task Registrera<T>(
            AggregatStrömIdentifierare<T> ström,
            IHändelse händelse,
            HändelseModus modus
        )
            where T : IAggregatRot
        {
            await using var session = _store.LightweightSession();
            if (modus == HändelseModus.SkapaNy)
            {
                _ = session.Events.StartStream(ström.ByggStrömIdentifierare(), händelse);
            }
            else if (modus == HändelseModus.LäggTill)
            {
                _ = session.Events.Append(ström.ByggStrömIdentifierare(), händelse);
            }

            await session.SaveChangesAsync();
        }
    }
}
