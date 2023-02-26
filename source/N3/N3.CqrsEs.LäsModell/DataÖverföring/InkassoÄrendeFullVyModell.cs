using N3.Modell;
using SmartAnalyzers.CSharpExtensions.Annotations;

namespace N3.CqrsEs.LäsModell.DataÖverföring
{
    [InitRequired]
    public class InkassoÄrendeFullVyModell
    {
        public UnikIdentifierare ÄrendeIdentifierare { get; init; }
        public UnikIdentifierare KlientReferens { get; init; }
        public UnikIdentifierare[] GäldenärsReferenser { get; init; }
        public Faktura[] Fakturor { get; init; }
    }
}
