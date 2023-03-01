namespace N3.Modell
{
    public record Faktura(
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
    )
    {
        public Faktura()
            : this(
                "",
                "",
                DateOnly.MinValue,
                DateOnly.MinValue,
                new Pengar(0, ""),
                new Pengar(0, ""),
                new Procent(0),
                RänteUträkningsSätt.DagligUträkningPåÅrsbasis,
                RänteSatsTyp.Fixerad
            ) { }
    };
}
