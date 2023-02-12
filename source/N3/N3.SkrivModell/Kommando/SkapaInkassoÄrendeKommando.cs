using CQRSlite.Commands;
using N3.Modell;

namespace N3.SkrivModell.Kommando
{
	public class SkapaInkassoÄrendeKommando : ICommand
	{
		public SkapaInkassoÄrendeKommando(UnikIdentifierare identifierare, UnikIdentifierare klientReferens)
		{
			Identifierare = identifierare;
			KlientReferens = klientReferens;
		}

		public UnikIdentifierare Identifierare { get; }
		public UnikIdentifierare KlientReferens { get; }
	}
}
