using N3.Modell;
using SmartAnalyzers.CSharpExtensions.Annotations;

namespace N3.CqrsEs.LäsModell.DataÖverföring
{
    [InitRequired]
    public class InkassoÄrendeFullVyModell
    {
        public UnikIdentifierare ÄrendeIdentifierare { get; init; }
        public UnikIdentifierare KlientReferens { get; set; }
        public UnikIdentifierare[] GäldenärsReferenser { get; set; }
        public Faktura[] Fakturor { get; set; }

        public int? ÄrendeNummer { get; set; }
    }
}
