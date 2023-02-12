namespace N3.Modell
{
	public readonly record struct BankKontoTyp(string Namn)
	{
		public static readonly BankKontoTyp BankGiro = new("BankGiro");
		public static readonly BankKontoTyp PlusGiro = new("PlusGiro");
		public static readonly BankKontoTyp BankKonto = new("Bankkonto");
	}
}