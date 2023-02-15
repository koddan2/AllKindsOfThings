using CQRSlite.Events;
using CQRSlite.Queries;
using N3.CqrsEs.Händelser;
using N3.CqrsEs.LäsModell.DataÖverföring;
using N3.CqrsEs.LäsModell.Frågor;
using N3.CqrsEs.LäsModell.Infrastruktur;

namespace N3.CqrsEs.LäsModell.HändelseHantering
{
    public class InkassoÄrendeDetaljVy :
        ICancellableEventHandler<InkassoÄrendeSkapades>
    {
        private readonly IVyLagring _vyLagring;

        public InkassoÄrendeDetaljVy(IVyLagring vyLagring)
        {
            _vyLagring = vyLagring;
        }

        public async Task Handle(InkassoÄrendeSkapades message, CancellationToken token = default)
        {
            var ärende = new DataÖverföring.InkassoÄrendeFullVyModell
            {
                Fakturor = message.Fakturor,
                GäldenärsReferenser = message.GäldenärsReferenser,
                KlientReferens = message.KlientReferens,
                ÄrendeIdentifierare = message.Id,
            };

            await _vyLagring.LäggTillÄrende(ärende);
        }
    }
}
