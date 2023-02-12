namespace N3.Modell
{
	public readonly record struct AvtalOmIndrivningsTjänst(
		string Namn,
		AvräkningsOrdning AvräkningsOrdning,
		ProvisionsModell ProvisionsModell,
		PrisLista PrisLista);
}