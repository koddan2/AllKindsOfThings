namespace N3.Modell
{
    public readonly record struct Faktura(
        string FakturaNummer,
        string UrsprungligBetalReferens,
        DateOnly FakturaDatum,
        DateOnly FörfalloDatum,
        Pengar UrsprungligtKapitalBelopp,
        Pengar KvarvarandeKapitalBelopp,
        Procent RänteSats,
        RänteUträkningsSätt RänteUträkningsSätt,
        RänteSatsTyp RänteSatsTyp,
        DateOnly? RänteStoppsDatum = null,
        Pengar? KvarvarandePåminnelseKostnad = null
    );
}
