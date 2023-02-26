using N3.CqrsEs.Ramverk;
using N3.Modell;

namespace N3.CqrsEs.SkrivModell.Kommando
{
    public class SkapaInkassoÄrendeKommando : IKommando
    {
        public SkapaInkassoÄrendeKommando(
            UnikIdentifierare identifierare,
            UnikIdentifierare klientReferens,
            UnikIdentifierare[] gäldenärsReferenser,
            Faktura[] fakturor
        )
        {
            Identifierare = identifierare;
            KlientReferens = klientReferens;
            GäldenärsReferenser = gäldenärsReferenser;
            Fakturor = fakturor;

            Rsn = identifierare;
            AuthenticationToken = identifierare;
            Frameworks = Array.Empty<string>();
            OriginatingFramework = "";
        }

        public UnikIdentifierare Identifierare { get; }
        public UnikIdentifierare KlientReferens { get; }
        public UnikIdentifierare[] GäldenärsReferenser { get; }
        public Faktura[] Fakturor { get; }

        public Guid Id { get; set; }
        public int ExpectedVersion { get; set; }
        public string AuthenticationToken { get; set; }
        public Guid CorrelationId { get; set; }
        public string OriginatingFramework { get; set; }
        public IEnumerable<string> Frameworks { get; set; }
        public Guid Rsn { get; set; }
    }
}
