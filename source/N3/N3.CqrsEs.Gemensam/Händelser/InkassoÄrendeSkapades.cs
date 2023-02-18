using N3.CqrsEs.Ramverk;
using N3.Modell;

namespace N3.CqrsEs.Gemensam.Händelser
{
    public sealed class InkassoÄrendeSkapades : IHändelse
    {
        public InkassoÄrendeSkapades(
            UnikIdentifierare identifierare,
            UnikIdentifierare klientReferens,
            UnikIdentifierare[] gäldenärsReferenser,
            Faktura[] fakturor,
            long ärendeNummer)
        {
            Id = identifierare;
            KlientReferens = klientReferens;
            GäldenärsReferenser = gäldenärsReferenser;
            Fakturor = fakturor;
            ÄrendeNummer = ärendeNummer;
        }

        public UnikIdentifierare KorrelationsIdentifierare { get; init; }
        public IEnumerable<string>? Kedja { get; set; }

        public string AggregatNamn => "InkassoÄrende";

        public UnikIdentifierare Id { get; }
        public int Version { get; }
        public DateTimeOffset TimeStamp { get; }

        public UnikIdentifierare KlientReferens { get; }
        public UnikIdentifierare[] GäldenärsReferenser { get; }
        public Faktura[] Fakturor { get; }
        public long ÄrendeNummer { get; }

    }
}
