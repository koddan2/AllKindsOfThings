using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace N3.Modell
{
    public record SvenskaKronorOchMoms(SvenskaKronor MomsBas, Moms Moms)
    {
        public SvenskaKronor TotalBelopp => MomsBas.LäggPå(Moms);
    }

    [DataContract]
    public record SvenskaKronor : Pengar
    {
        public SvenskaKronor(decimal belopp)
            : base(belopp, "SEK") { }

        public override string ToString() => $"{Belopp} kr";

        [JsonIgnore]
        public SvenskaKronor AvrundaHelaKronor =>
            Math.Round(Belopp, 0, MidpointRounding.AwayFromZero);

        [JsonIgnore]
        public SvenskaKronor AvrundaHelaKronorOchÖren =>
            Math.Round(Belopp, 2, MidpointRounding.AwayFromZero);

        public static implicit operator decimal(SvenskaKronor u) => u.Belopp;

        public static implicit operator SvenskaKronor(decimal d) => new(d);
    }
}
