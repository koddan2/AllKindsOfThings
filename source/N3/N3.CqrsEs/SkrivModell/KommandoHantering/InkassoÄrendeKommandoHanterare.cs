using CQRSlite.Commands;
using CQRSlite.Domain;
using CQRSlite.Events;
using N3.CqrsEs.SkrivModell.Domän;
using N3.CqrsEs.SkrivModell.Kommando;

namespace N3.CqrsEs.SkrivModell.Hantering
{
    public class InkassoÄrendeKommandoHanterare : ICommandHandler<SkapaInkassoÄrendeKommando>
    {
        private readonly ISession _session;
        private readonly IÄrendeNummerUträknare _ärendeNummerUträknare;
        private readonly IEventStore _eventStore;

        public InkassoÄrendeKommandoHanterare(
            IInkassoÄrendeSession session,
            IÄrendeNummerUträknare ärendeNummerUträknare,
            IEventStore eventStore)
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

            var events = await _eventStore.Get(message.Identifierare, -1);
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

            await _session.Add(ärende);
            await _session.Commit();
        }
    }

    [Serializable]
    public class AggregatExisterarRedanException : Exception
    {
        public AggregatExisterarRedanException() { }
        public AggregatExisterarRedanException(string message) : base(message) { }
        public AggregatExisterarRedanException(string message, Exception inner) : base(message, inner) { }
        protected AggregatExisterarRedanException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public Guid Id { get; init; }
    }
}
