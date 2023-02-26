using N3.CqrsEs.LäsModell.DataÖverföring;
using N3.CqrsEs.LäsModell.Frågor;
using N3.CqrsEs.LäsModell.Infrastruktur;

namespace N3.CqrsEs.Test.TestTjänster
{
    public class TestFrågeHanterare
        : IFrågeHanterare<HämtaSpecifiktInkassoÄrende, InkassoÄrendeFullVyModell>
    {
        private readonly IVyLagring _vyLagring;

        public TestFrågeHanterare(IVyLagring vyLagring)
        {
            _vyLagring = vyLagring;
        }

        public async Task<InkassoÄrendeFullVyModell> Hantera(
            HämtaSpecifiktInkassoÄrende fråga,
            CancellationToken cancellationToken
        )
        {
            return await _vyLagring.HämtaSpecifiktÄrende(fråga, cancellationToken);
        }
    }
}
