using CQRSlite.Commands;
using N3.Modell;

namespace N3.CqrsEs.SkrivModell.Kommando
{
	public class SkapaInkassoÄrendeKommando : ICommand
	{
		public SkapaInkassoÄrendeKommando(
			UnikIdentifierare identifierare,
			UnikIdentifierare klientReferens,
			UnikIdentifierare[] gäldenärsReferenser,
			Faktura[] fakturor)
		{
			Identifierare = identifierare;
			KlientReferens = klientReferens;
			GäldenärsReferenser = gäldenärsReferenser;
			Fakturor = fakturor;
		}

		public UnikIdentifierare Identifierare { get; }
		public UnikIdentifierare KlientReferens { get; }
		public UnikIdentifierare[] GäldenärsReferenser { get; }
		public Faktura[] Fakturor { get; }
	}
}
