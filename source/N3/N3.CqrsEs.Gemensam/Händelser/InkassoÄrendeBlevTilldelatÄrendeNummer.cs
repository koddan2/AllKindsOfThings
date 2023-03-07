using N3.CqrsEs.Ramverk;
using SmartAnalyzers.CSharpExtensions.Annotations;
using System.Text.Json.Serialization;

namespace N3.CqrsEs.Gemensam.Händelser
{
    [InitRequired]
    public sealed class InkassoÄrendeBlevTilldelatÄrendeNummer : IAggregatHändelse
    {
        public InkassoÄrendeBlevTilldelatÄrendeNummer(
            string aggregatIdentifierare,
            int ärendeNummer
        )
        {
            AggregatIdentifierare = aggregatIdentifierare;
            ÄrendeNummer = ärendeNummer;
        }

        public string KorrelationsIdentifierare { get; init; }

        [JsonIgnore]
        public string AggregatIdentifierare { get; init; }
        public int ÄrendeNummer { get; init; }
    }
}
