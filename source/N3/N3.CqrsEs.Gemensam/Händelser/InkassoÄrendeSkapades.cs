using N3.CqrsEs.Ramverk;
using N3.Modell;

namespace N3.CqrsEs.Gemensam.Händelser
{
    public sealed class InkassoÄrendeSkapades : IAggregatHändelse
    {
        public InkassoÄrendeSkapades(
            UnikIdentifierare aggregatIdentifierare,
            UnikIdentifierare klientReferens,
            UnikIdentifierare[] gäldenärsReferenser,
            Faktura[] fakturor,
            long ärendeNummer
        )
        {
            AggregatIdentifierare = aggregatIdentifierare;
            KlientReferens = klientReferens;
            GäldenärsReferenser = gäldenärsReferenser;
            Fakturor = fakturor;
            ÄrendeNummer = ärendeNummer;
        }

        public UnikIdentifierare KorrelationsIdentifierare { get; init; }
        public IEnumerable<string> Historia { get; } = new List<string>();

        public UnikIdentifierare AggregatIdentifierare { get; }
        public long Revision { get; }
        public DateTimeOffset Tidsstämpel { get; }

        public UnikIdentifierare KlientReferens { get; }
        public UnikIdentifierare[] GäldenärsReferenser { get; }
        public Faktura[] Fakturor { get; }
        public long ÄrendeNummer { get; }
    }
}
