using Cqrs.Commands;
using Cqrs.Domain;
using Cqrs.Events;
using Cqrs.Messages;
using N3.CqrsEs.SkrivModell.Domän;
using N3.CqrsEs.SkrivModell.Kommando;

namespace N3.CqrsEs.SkrivModell.Hantering
{
    public class InkassoÄrendeKommandoHanterare : ICommandHandler<string, SkapaInkassoÄrendeKommando>
    {
        private readonly IUnitOfWork<string> _session;
        private readonly IÄrendeNummerUträknare _ärendeNummerUträknare;
        private readonly IEventStore<string> _eventStore;

        public InkassoÄrendeKommandoHanterare(
            IUnitOfWork<string> session,
            IÄrendeNummerUträknare ärendeNummerUträknare,
            IEventStore<string> eventStore)
        {
            _session = session;
            _ärendeNummerUträknare = ärendeNummerUträknare;
            _eventStore = eventStore;
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

            var events =  _eventStore.Get(message.Identifierare);
            if (events.Any())
            {
                throw new AggregatExisterarRedanException("Kan inte skapa ett ärende med en unik identifierare som redan finns.") { Id = message.Identifierare };
            }

            var ärendeNr = await _ärendeNummerUträknare.TaFramNästaLedigaÄrendeNummer();
            var ärende = new InkassoÄrende(
                message.Identifierare,
                message.KlientReferens,
                gäldenärsReferenser: message.GäldenärsReferenser,
                fakturor: message.Fakturor,
                ärendeNummer: ärendeNr);

            _session.Add(ärende);
            _session.Commit();
        }

        void IMessageHandler<SkapaInkassoÄrendeKommando>.Handle(SkapaInkassoÄrendeKommando message)
        {
            throw new NotImplementedException();
        }
    }
}
