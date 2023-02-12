using N3.Modell;

namespace N3.SkrivModell.Händelser
{
	public sealed class InkassoÄrendeSkapades : IInkassoHändelse
	{
		public InkassoÄrendeSkapades(
			UnikIdentifierare identifierare, 
			UnikIdentifierare klientReferens,
			UnikIdentifierare[] gäldenärsReferenser,
			Faktura[] fakturor,
			long ärendeNummer)
		{
			Id = identifierare;
			KlientReferens = klientReferens;
			GäldenärsReferenser = gäldenärsReferenser;
			Fakturor = fakturor;
			ÄrendeNummer = ärendeNummer;
		}

		public string AggregateName => "InkassoÄrende";
		public Guid Id { get; set; }
		public int Version { get; set; }
		public DateTimeOffset TimeStamp { get; set; }

		public UnikIdentifierare KlientReferens { get; }
		public UnikIdentifierare[] GäldenärsReferenser { get; }
		public Faktura[] Fakturor { get; }
		public long ÄrendeNummer { get; }
	}
}
