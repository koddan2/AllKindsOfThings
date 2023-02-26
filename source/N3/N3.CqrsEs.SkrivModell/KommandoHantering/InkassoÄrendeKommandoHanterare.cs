using N3.CqrsEs.Ramverk;
using N3.CqrsEs.SkrivModell.Domän;
using N3.CqrsEs.SkrivModell.Kommando;

namespace N3.CqrsEs.SkrivModell.KommandoHantering
{
    public class InkassoÄrendeKommandoHanterare : IKommandoHanterare<SkapaInkassoÄrendeKommando>
    {
        private readonly IÄrendeNummerUträknare _ärendeNummerUträknare;
        private readonly IHändelseKassa _händelseKassa;

        public InkassoÄrendeKommandoHanterare(
            IHändelseKassa händelseKassa,
            IÄrendeNummerUträknare ärendeNummerUträknare
        )
        {
            _ärendeNummerUträknare = ärendeNummerUträknare;
            _händelseKassa = händelseKassa;
        }

        public async Task Hantera(SkapaInkassoÄrendeKommando meddelande)
        {
            ////try
            ////{
            ////    var existerar = await _session.Get<InkassoÄrende?>(message.Identifierare);
            ////    if (existerar is not null)
            ////    {
            ////        // ...
            ////        throw new AggregatExisterarRedanException("Kan inte skapa ett ärende med en unik identifierare som redan finns.") { Id = message.Identifierare };
            ////    }
            ////}
            ////catch (Exception)
            ////{
            ////    // OK
            ////}

            var events = _händelseKassa.Hämta(
                new AggregatStrömIdentifierare<InkassoÄrende>(meddelande.Identifierare)
            );
            if (events.Any())
            {
                throw new AggregatExisterarRedanException(
                    "Kan inte skapa ett ärende med en unik identifierare som redan finns."
                )
                {
                    Id = meddelande.Identifierare
                };
            }

            var ärendeNr = await _ärendeNummerUträknare.TaFramNästaLedigaÄrendeNummer();
            var ärende = new InkassoÄrende(meddelande.Identifierare);
            await ärende.SkapaÄrende(
                _händelseKassa,
                meddelande.KlientReferens,
                gäldenärsReferenser: meddelande.GäldenärsReferenser,
                fakturor: meddelande.Fakturor,
                ärendeNummer: ärendeNr
            );

            ////_session.Add(ärende);
            ////_session.Commit();
        }
    }
}
