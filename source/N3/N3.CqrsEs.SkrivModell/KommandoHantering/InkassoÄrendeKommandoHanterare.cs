using N3.CqrsEs.Ramverk;
using N3.CqrsEs.SkrivModell.Domän;
using N3.CqrsEs.SkrivModell.Kommando;

namespace N3.CqrsEs.SkrivModell.KommandoHantering
{
    public class InkassoÄrendeKommandoHanterare
        : IKommandoHanterare<SkapaInkassoÄrendeKommando>,
            IKommandoHanterare<TilldelaÄrendeNummerTillInkassoÄrendeKommando>
    {
        private readonly IHändelseKassa _händelseKassa;
        private readonly IAggregateRepository _repo;

        public InkassoÄrendeKommandoHanterare(
            IHändelseKassa händelseKassa,
            IAggregateRepository repo
        )
        {
            _händelseKassa = händelseKassa;
            _repo = repo;
        }

        public async Task Hantera(SkapaInkassoÄrendeKommando kommando)
        {
            var händelser = await _händelseKassa.Hämta(
                new AggregatStrömIdentifierare<InkassoÄrende>(kommando.AggregatIdentifierare)
            );
            if (händelser.Any())
            {
                throw new AggregatExisterarRedanException(
                    "Kan inte skapa ett ärende med en unik identifierare som redan finns."
                )
                {
                    Aggregat = typeof(InkassoÄrende).Name,
                    AggregatIdentifierare = kommando.AggregatIdentifierare,
                };
            }

            var ärende = new InkassoÄrende(kommando.AggregatIdentifierare);
            ärende.SkapaÄrende(
                kommando.KlientReferens,
                gäldenärsReferenser: kommando.GäldenärsReferenser,
                fakturor: kommando.Fakturor
            );
            await _repo.StoreAsync(ärende, newStream: true);
        }

        public async Task Hantera(TilldelaÄrendeNummerTillInkassoÄrendeKommando kommando)
        {
            var händelser = await _händelseKassa.Hämta(
                new AggregatStrömIdentifierare<InkassoÄrende>(kommando.AggregatIdentifierare)
            );
            if (!händelser.Any())
            {
                throw new AggregatHarInteSkapatsException()
                {
                    Aggregat = typeof(InkassoÄrende).Name,
                    AggregatIdentifierare = kommando.AggregatIdentifierare
                };
            }
            var ärende = new InkassoÄrende(kommando.AggregatIdentifierare);
            ärende.Ladda(händelser);
            ärende.TilldelaÄrendeNummer(kommando.ÄrendeNummer);
            await _repo.StoreAsync(ärende);
        }
    }
}
