namespace N3.Modell
{
    public readonly record struct SkuldElement(
        string Namn,
        BeloppsTyp Typ,
        IPengar? StandardBelopp = null
    )
    {
        public static readonly SkuldElement Kapital = new("Kapital", BeloppsTyp.HuvudsakligSkuld);
        public static readonly SkuldElement RäntaPåKapital =
            new("Ränta på kapital", BeloppsTyp.Ränta);
        public static readonly SkuldElement PåminnelseKostnad =
            new("Påminnelsekostnad", BeloppsTyp.Kostnad, new SvenskaKronor(60));
        public static readonly SkuldElement InkssoKostnad =
            new("Inkssokostnad", BeloppsTyp.Kostnad, new SvenskaKronor(180));

        public static readonly SkuldElement BfAnsökningsAvgift =
            new(
                "Betalningsföreläggande, ansökningsAvgift",
                BeloppsTyp.Kostnad,
                new SvenskaKronor(300)
            );
        public static readonly SkuldElement BfArvode =
            new("Betalningsföreläggande, arvode", BeloppsTyp.Kostnad, new SvenskaKronor(380));
        public static readonly SkuldElement BfArvodeVidAvhysning =
            new(
                "Betalningsföreläggande, arvode vid avhysning",
                BeloppsTyp.Kostnad,
                new SvenskaKronor(420)
            );

        public static readonly SkuldElement VsFullständig =
            new("Verkställighet, fullständig", BeloppsTyp.Kostnad, new SvenskaKronor(600));
    }
}
