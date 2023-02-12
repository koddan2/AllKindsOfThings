using System;
using CQRSlite.Commands;
using CQRSlite.Domain;
using N3.Modell;
using N3.SkrivModell.Domän;

namespace N3.SkrivModell.Kommando
{
	public class InkassoÄrendeKommandoHanterare : ICommandHandler<SkapaInkassoÄrendeKommando>
	{
		private readonly ISession _session;
		private readonly IÄrendeNummerUträknare _ärendeNummerUträknare;

		public InkassoÄrendeKommandoHanterare(ISession session, IÄrendeNummerUträknare ärendeNummerUträknare)
		{
			_session = session;
			_ärendeNummerUträknare = ärendeNummerUträknare;
		}

		public async Task Handle(SkapaInkassoÄrendeKommando message)
		{
			var ärendeNr = await _ärendeNummerUträknare.TaFramNästaLedigaÄrendeNummer();
			var ärende = new InkassoÄrende(
				message.Identifierare,
				message.KlientReferens,
				Array.Empty<UnikIdentifierare>(),
				Array.Empty<Faktura>(),
				ärendeNummer: ärendeNr);
			await _session.Add(ärende);
			await _session.Commit();
		}
	}
}
