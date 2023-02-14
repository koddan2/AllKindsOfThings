using CQRSlite.Events;
using N3.CqrsEs.Händelser;
using N3.CqrsEs.LäsModell.Infrastruktur;

namespace N3.CqrsEs.LäsModell.HändelseHantering
{
	////public class InkassoÄrendeListVy : ICancellableEventHandler<InkassoÄrendeSkapades>
	////{
	////	private readonly IVyLagring _vyLagring;

	////	public InkassoÄrendeListVy(IVyLagring vyLagring)
	////	{
	////		_vyLagring = vyLagring;
	////	}

	////	public async Task Handle(InkassoÄrendeSkapades message, CancellationToken token = default)
	////	{
	////		await _vyLagring.LäggTillÄrende(new DataÖverföring.InkassoÄrendeFullVyModell
	////		{
	////			ÄrendeIdentifierare = message.Id,
	////			KlientReferens = message.KlientReferens,
	////			GäldenärsReferenser = message.GäldenärsReferenser,
	////			Fakturor = message.Fakturor,
	////		}, token);
	////	}
	////}
}
