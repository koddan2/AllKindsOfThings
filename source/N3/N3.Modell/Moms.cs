namespace N3.Modell
{
	public readonly record struct Moms(Procent Procent)
	{
		public static Moms _25 => new(25);
		public static Moms _12 => new(12);
		public static Moms _6 => new(6);
		public static Moms _0 => new(0);
	}

	public static class MomsExtraFunktioner
	{
		public static SvenskaKronor LäggPå(this SvenskaKronor sek, Moms moms)
			=> sek with { Belopp = sek.Belopp * (1 + moms.Procent.Faktor) };

		public static SvenskaKronor RäknaUtMomsDel(this SvenskaKronor sek, Moms moms)
			=> sek with { Belopp = sek.Belopp is 0 ? 0 : sek.Belopp * moms.Procent.Faktor };

		public static SvenskaKronor RäknaUtMomsBas(this SvenskaKronor sek, Moms moms)
			=> sek with { Belopp = sek.Belopp is 0 ? 0 : sek.Belopp / (1 + moms.Procent.Faktor) };
	}
}