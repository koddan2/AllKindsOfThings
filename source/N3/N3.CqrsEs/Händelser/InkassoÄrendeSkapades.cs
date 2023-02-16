using N3.Modell;

namespace N3.CqrsEs.Händelser
{
    public sealed class InkassoÄrendeSkapades : IInkassoHändelse
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

        public string AggregatNamn => "InkassoÄrende";

        public Guid Id { get; set; }
        public int Version { get; set; }
        public DateTimeOffset TimeStamp { get; set; }

        public UnikIdentifierare KlientReferens { get; }
        public UnikIdentifierare[] GäldenärsReferenser { get; }
        public Faktura[] Fakturor { get; }
        public long ÄrendeNummer { get; }

        public string AuthenticationToken { get; set; } = "";
        public Guid CorrelationId { get; set; } = Guid.Empty;
        public string OriginatingFramework { get; set; } = "";
        public IEnumerable<string> Frameworks { get; set; } = Array.Empty<string>();
    }
}
