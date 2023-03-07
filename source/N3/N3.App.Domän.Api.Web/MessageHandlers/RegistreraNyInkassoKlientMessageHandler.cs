using Marten;
using N3.App.Domän.Api.Web.Messages;
using N3.CqrsEs.SkrivModell.Domän;
using Rebus.Bus;
using Rebus.Handlers;

namespace N3.App.Domän.Api.Web.MessageHandlers
{
    public class RegistreraNyInkassoKlientMessageHandler
        : IHandleMessages<RegistreraNyInkassoKlientKommando>
    {
        private readonly IBus _bus;
        private readonly IDocumentStore _store;
        private readonly AggregateRepository _aggregateRepository;

        public RegistreraNyInkassoKlientMessageHandler(
            IBus bus,
            IDocumentStore store,
            AggregateRepository aggregateRepository
        )
        {
            _bus = bus;
            _store = store;
            _aggregateRepository = aggregateRepository;
        }

        public async Task Handle(RegistreraNyInkassoKlientKommando message)
        {
            var modell = message.Data;
            using var session = _store.LightweightSession();
            if (modell.Id is string id)
            {
                if (await _aggregateRepository.ExistsAsync<InkassoKlient>(id))
                {
                    throw new InvalidOperationException(
                        $"InkassoKlient med Id={id} existerar redan."
                    );
                }
            }
            var aggregate = new InkassoKlient(modell.Id ?? UnikIdentifierare.Skapa());
            aggregate.SkapaKlient(modell.Namn);
            await _aggregateRepository.StoreAsync(aggregate, newStream: true);
        }
    }
}
