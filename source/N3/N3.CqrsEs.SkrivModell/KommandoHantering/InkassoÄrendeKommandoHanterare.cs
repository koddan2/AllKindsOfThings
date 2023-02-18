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

        public async Task Handle(SkapaInkassoÄrendeKommando message)
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

            var events = _händelseKassa.Hämta(message.Identifierare);
            if (events.Any())
            {
                throw new AggregatExisterarRedanException("Kan inte skapa ett ärende med en unik identifierare som redan finns.") { Id = message.Identifierare };
            }

            var ärendeNr = await _ärendeNummerUträknare.TaFramNästaLedigaÄrendeNummer();
            var ärende = new InkassoÄrende(message.Identifierare);
            await ärende.SkapaÄrende(
                _händelseKassa,
                message.KlientReferens,
                gäldenärsReferenser: message.GäldenärsReferenser,
                fakturor: message.Fakturor,
                ärendeNummer: ärendeNr);

            ////_session.Add(ärende);
            ////_session.Commit();
        }

        public Task Hantera(SkapaInkassoÄrendeKommando kommando)
        {
            throw new NotImplementedException();
        }
    }
}
