namespace N3.Modell
{
	public readonly record struct Faktura(
		string FakturaNummer,
		string UrsprungligBetalReferens,
		DateOnly FakturaDatum,
		DateOnly FörfalloDatum,
		IPengar UrsprungligtKapitalBelopp,
		IPengar KvarvarandeKapitalBelopp,
		Procent RänteSats,
		RänteUträkningsSätt RänteUträkningsSätt,
		RänteSatsTyp RänteSatsTyp,
		SkuldElement[] Kostnader,
		DateOnly? RänteStoppsDatum = null
		);
}