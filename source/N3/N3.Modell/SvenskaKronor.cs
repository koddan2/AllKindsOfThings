using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace N3.Modell
{
    public readonly record struct SvenskaKronorOchMoms(SvenskaKronor MomsBas, Moms Moms)
    {
        public SvenskaKronor TotalBelopp => MomsBas.LäggPå(Moms);
    }

    [DataContract]
    public readonly record struct SvenskaKronor([property: DataMember] decimal Belopp) : IPengar
    {
        [DataMember]
        public string ValutaKod => "SEK";

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
