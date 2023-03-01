using N3.CqrsEs.Ramverk;
using SmartAnalyzers.CSharpExtensions.Annotations;

namespace N3.CqrsEs.Gemensam.Händelser
{
    [InitRequired]
    public sealed class InkassoÄrendeBlevTilldelatÄrendeNummer : IAggregatHändelse
    {
        public InkassoÄrendeBlevTilldelatÄrendeNummer(
            UnikIdentifierare aggregatIdentifierare,
            int ärendeNummer
        )
        {
            AggregatIdentifierare = aggregatIdentifierare;
            ÄrendeNummer = ärendeNummer;
        }

        public UnikIdentifierare KorrelationsIdentifierare { get; init; }
        public IEnumerable<string> Historia { get; } = new List<string>();

        public UnikIdentifierare AggregatIdentifierare { get; init; }
        public long Revision { get; init; }
        public DateTimeOffset Tidsstämpel { get; init; }

        public int ÄrendeNummer { get; init; }
    }
}
