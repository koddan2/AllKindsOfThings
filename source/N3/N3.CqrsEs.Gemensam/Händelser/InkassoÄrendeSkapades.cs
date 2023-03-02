using N3.CqrsEs.Ramverk;
using N3.Modell;
using SmartAnalyzers.CSharpExtensions.Annotations;

namespace N3.CqrsEs.Gemensam.Händelser
{
    [InitRequired]
    public sealed class InkassoKlientSkapades : IAggregatHändelse
    {
        public InkassoKlientSkapades(
            string aggregatIdentifierare,
            string fullkomligtNamn
        )
        {
            AggregatIdentifierare = aggregatIdentifierare;
            FullkomligtNamn = fullkomligtNamn;
        }

        public string KorrelationsIdentifierare { get; init; }
        public IEnumerable<string> Historia { get; } = new List<string>();

        public string AggregatIdentifierare { get; }
        public long Revision { get; }
        public DateTimeOffset Tidsstämpel { get; }

        public string FullkomligtNamn { get; }
    }

    [InitRequired]
    public sealed class InkassoÄrendeSkapades : IAggregatHändelse
    {
        public InkassoÄrendeSkapades(
            string aggregatIdentifierare,
            string klientReferens,
            string[] gäldenärsReferenser,
            Faktura[] fakturor
        )
        {
            AggregatIdentifierare = aggregatIdentifierare;
            KlientReferens = klientReferens;
            GäldenärsReferenser = gäldenärsReferenser;
            Fakturor = fakturor;
        }

        public string KorrelationsIdentifierare { get; init; }
        public IEnumerable<string> Historia { get; } = new List<string>();

        public string AggregatIdentifierare { get; }
        public long Revision { get; }
        public DateTimeOffset Tidsstämpel { get; }

        public string KlientReferens { get; }
        public string[] GäldenärsReferenser { get; }
        public Faktura[] Fakturor { get; }
    }
}
