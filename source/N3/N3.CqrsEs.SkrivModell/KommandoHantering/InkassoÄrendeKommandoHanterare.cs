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

        public InkassoÄrendeKommandoHanterare(IHändelseKassa händelseKassa)
        {
            _händelseKassa = händelseKassa;
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
            await ärende.SkapaÄrende(
                _händelseKassa,
                kommando.KlientReferens,
                gäldenärsReferenser: kommando.GäldenärsReferenser,
                fakturor: kommando.Fakturor
            );
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
            await ärende.TilldelaÄrendeNummer(_händelseKassa, kommando.ÄrendeNummer);
        }
    }
}
