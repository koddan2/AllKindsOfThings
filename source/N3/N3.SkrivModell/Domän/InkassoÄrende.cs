using CQRSlite.Domain;
using N3.Modell;
using N3.SkrivModell.Händelser;

namespace N3.SkrivModell.Domän
{
	public sealed class InkassoÄrende : AggregateRoot
	{
		private InkassoÄrende() { }

		public InkassoÄrende(
			UnikIdentifierare identifierare, 
			UnikIdentifierare klientReferens, 
			UnikIdentifierare[] gäldenärsReferenser,
			Faktura[] fakturor,
			long ärendeNummer)
		{
			Id = identifierare;
			ApplyChange(new InkassoÄrendeSkapades(identifierare, klientReferens, gäldenärsReferenser, fakturor, ärendeNummer));
		}
	}
}