using CQRSlite.Events;
using N3.CqrsEs.Händelser;
using N3.CqrsEs.LäsModell.Infrastruktur;

namespace N3.CqrsEs.LäsModell.HändelseHantering
{
	public class InkassoÄrendeDetaljVy : ICancellableEventHandler<InkassoÄrendeSkapades>
	{
		private readonly IVyLagring _vyLagring;

		public InkassoÄrendeDetaljVy(IVyLagring vyLagring)
		{
			_vyLagring = vyLagring;
		}

		public Task Handle(InkassoÄrendeSkapades message, CancellationToken token = default)
		{
			throw new NotImplementedException();
		}
	}
}
