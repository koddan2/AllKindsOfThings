namespace N3.Modell
{
	public readonly record struct Faktura(
		string FakturaNummer,
		string UrsprungligBetalReferens,
		IPengar UrsprungligtKapitalBelopp,
		IPengar KvarvarandeKapitalBelopp,
		Procent RänteSats,
		RänteUträkningsSätt RänteUträkningsSätt,
		RänteSatsTyp RänteSatsTyp,
		DateTimeOffset RänteStoppsDatum,
		SkuldElement[] Kostnader);
}