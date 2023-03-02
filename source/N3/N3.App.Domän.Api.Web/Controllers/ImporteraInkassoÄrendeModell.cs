using N3.CqrsEs.Ramverk;
using N3.CqrsEs.SkrivModell.Anhopning;
using Rebus.Handlers;

namespace N3.App.Domän.Api.Web.Controllers
{
    public class ImporteraInkassoÄrendeHanterare : IHandleMessages<ImporteraInkassoÄrendeModell>
    {
        public async Task Handle(ImporteraInkassoÄrendeModell message)
        {
            await ValueTask.CompletedTask;
            Console.WriteLine("OK: {0}", message.AktivitetsIdentifierare);
        }
    }
    public class ImporteraInkassoÄrendeHanterare2 : IHandleMessages<ImporteraInkassoÄrendeModell>
    {
        public async Task Handle(ImporteraInkassoÄrendeModell message)
        {
            await ValueTask.CompletedTask;
            Console.WriteLine("OK II: {0}", message.AktivitetsIdentifierare);
        }
    }
    public record ImporteraInkassoÄrendeModell(
        UnikIdentifierare AktivitetsIdentifierare,
        ÄrendeImportModell ÄrendeImportModell
    ) : IAktivitet
    {
        public const string Kategori = "ImporteraInkassoÄrende";
        public AktivitetsKategori AktivitetsKategori => new(Kategori);
    }
}
