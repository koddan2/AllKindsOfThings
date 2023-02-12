using CQRSlite.Commands;
using CQRSlite.Domain;
using N3.SkrivModell.Domän;

namespace N3.SkrivModell.Kommando
{
	public class InkassoÄrendeKommandoHanterare : ICommandHandler<SkapaInkassoÄrendeKommando>
	{
		private readonly ISession _session;

		public InkassoÄrendeKommandoHanterare(ISession session)
		{
			_session = session;
		}

		public async Task Handle(SkapaInkassoÄrendeKommando message)
		{
			var ärende = new InkassoÄrende(message.Identifierare, message.KlientReferens);
			await _session.Add(ärende);
			await _session.Commit();
		}
	}
}
