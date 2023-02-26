using N3.CqrsEs.Ramverk;
using SmartAnalyzers.CSharpExtensions.Annotations;

namespace N3.CqrsEs.SkrivModell.Kommando
{
    [InitRequired]
    public class TilldelaÄrendeNummerTillInkassoÄrendeKommando : IKommando
    {
        public TilldelaÄrendeNummerTillInkassoÄrendeKommando(
            UnikIdentifierare aggregatIdentifierare,
            int ärendeNummer
        )
        {
            AggregatIdentifierare = aggregatIdentifierare;
            ÄrendeNummer = ärendeNummer;
        }

        public UnikIdentifierare KorrelationsIdentifierare { get; init; }
        public IEnumerable<string> Historia { get; } = new List<string>();

        public string Auktorisering { get; init; }
        public long FörväntadRevision { get; init; }

        public UnikIdentifierare AggregatIdentifierare { get; }
        public int ÄrendeNummer { get; }
    }
}
