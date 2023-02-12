using N3.Modell;

namespace N3.SkrivModell.Händelser
{
	public sealed class InkassoÄrendeSkapades : IInkassoHändelse
	{
		public InkassoÄrendeSkapades(
			UnikIdentifierare identifierare, 
			UnikIdentifierare klientReferens,
			UnikIdentifierare[] gäldenärsReferenser,
			int ärendeNummer)
		{
			Id = identifierare;
			KlientReferens = klientReferens;
			GäldenärsReferenser = gäldenärsReferenser;
			ÄrendeNummer = ärendeNummer;
		}

		public string AggregateName => "InkassoÄrende";
		public Guid Id { get; set; }
		public int Version { get; set; }
		public DateTimeOffset TimeStamp { get; set; }

		public UnikIdentifierare KlientReferens { get; }
		public UnikIdentifierare[] GäldenärsReferenser { get; }
		public int ÄrendeNummer { get; }
	}
}
