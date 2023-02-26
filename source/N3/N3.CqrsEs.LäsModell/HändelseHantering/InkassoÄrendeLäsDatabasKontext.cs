using N3.CqrsEs.Gemensam.Händelser;
using N3.CqrsEs.LäsModell.DataÖverföring;
using N3.CqrsEs.LäsModell.Frågor;
using N3.CqrsEs.LäsModell.Infrastruktur;
using N3.CqrsEs.Ramverk;

namespace N3.CqrsEs.LäsModell.HändelseHantering
{
    public interface IHändelseMottagare<THändelse>
        where THändelse : IHändelse
    {
        Task TaEmot(THändelse händelse, CancellationToken cancellationToken = default);
    }

    public class InkassoÄrendeLäsDatabasKontext : IHändelseMottagare<InkassoÄrendeSkapades>
    {
        private readonly IVyLagring _vyLagring;

        public InkassoÄrendeLäsDatabasKontext(IVyLagring vyLagring)
        {
            _vyLagring = vyLagring;
        }

        public async Task<InkassoÄrendeFullVyModell> HämtaÄrende(
            HämtaSpecifiktInkassoÄrende parametrar,
            CancellationToken cancellationToken = default
        )
        {
            return await _vyLagring.HämtaSpecifiktÄrende(parametrar, cancellationToken);
        }

        public async Task TaEmot(
            InkassoÄrendeSkapades händelse,
            CancellationToken cancellationToken = default
        )
        {
            var ärende = new DataÖverföring.InkassoÄrendeFullVyModell
            {
                Fakturor = händelse.Fakturor,
                GäldenärsReferenser = händelse.GäldenärsReferenser,
                KlientReferens = händelse.KlientReferens,
                ÄrendeIdentifierare = händelse.Id,
            };

            await _vyLagring.LäggTillÄrende(ärende, cancellationToken);
        }
    }
}
