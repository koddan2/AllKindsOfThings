using Marten;
using N3.CqrsEs.SkrivModell.JobbPaket;
using SlimMessageBus;

namespace N3.App.Domän.Api.Web.MessageHandlers
{
    public class ImporteraInkassoÄrendeMessageHandler : IConsumer<ImporteraInkassoÄrendeModell>
    {
        private readonly IDocumentStore _store;
        private readonly IMessageBus _bus;

        public ImporteraInkassoÄrendeMessageHandler(
            ILogger<ImporteraInkassoÄrendeMessageHandler> logger,
            IDocumentStore store,
            IMessageBus bus
        )
        {
            _store = store;
            _bus = bus;
        }

        public async Task OnHandle(ImporteraInkassoÄrendeModell message)
        {
            using var session = _store.LightweightSession();
            var existing = await session
                .Query<ImporteraInkassoÄrendeModell>()
                .FirstOrDefaultAsync(x => x.Id == message.Id);
            if (existing is null)
            {
                session.Store(message);
                await session.SaveChangesAsync();
                await _bus.Publish(new ImportAvInkassoÄrendeKölagt(message.Id));
            }
        }
    }
}
