namespace N3.Modell
{
	public readonly record struct IndrivningsTjänst(
		string Namn,
		AvräkningsOrdning AvräkningsOrdning,
		ProvisionsModell ProvisionsModell,
		PrisLista PrisLista);
}