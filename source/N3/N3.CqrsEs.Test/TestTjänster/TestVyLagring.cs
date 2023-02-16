using N3.CqrsEs.LäsModell.DataÖverföring;
using N3.CqrsEs.LäsModell.Frågor;
using N3.CqrsEs.LäsModell.Infrastruktur;
using N3.CqrsEs.SkrivModell.Hantering;

namespace N3.CqrsEs.Test.TestTjänster
{
    internal class TestVyLagringDatabas
    {
        public readonly Dictionary<string, InkassoÄrendeFullVyModell> Ärenden = new();
    }

    internal class TestVyLagring :
        IVyLagring,
        IÄrendeNummerUträknare
    {
        private readonly TestVyLagringDatabas _databas;

        public TestVyLagring(TestVyLagringDatabas databas)
        {
            _databas = databas;
        }

        public async Task<InkassoÄrendeFullVyModell> Handle(HämtaSpecifiktInkassoÄrende message, CancellationToken token = default)
        {
            await ValueTask.CompletedTask;
            return _databas.Ärenden[message.ÄrendeIdentifierare];
        }

        public async Task<InkassoÄrendeFullVyModell> HämtaSpecifiktÄrende(HämtaSpecifiktInkassoÄrende parametrar, CancellationToken token = default)
        {
            await Task.CompletedTask;
            return _databas.Ärenden[parametrar.ÄrendeIdentifierare];
        }

        public async Task LäggTillÄrende(InkassoÄrendeFullVyModell inkassoÄrendeFullVyModell, CancellationToken token = default)
        {
            await Task.CompletedTask;
            _databas.Ärenden.Add(inkassoÄrendeFullVyModell.ÄrendeIdentifierare, inkassoÄrendeFullVyModell);
        }

        public async Task<long> TaFramNästaLedigaÄrendeNummer()
        {
            await Task.CompletedTask;
            return _databas.Ärenden.Values.Count + 1;
        }

    }
}