using N3.Modell;
using SmartAnalyzers.CSharpExtensions.Annotations;

namespace N3.CqrsEs.LäsModell.DataÖverföring
{
    [InitRequired]
    public class InkassoÄrendeFullVyModell
    {
        public string ÄrendeIdentifierare { get; init; }
        public string KlientReferens { get; set; }
        public string[] GäldenärsReferenser { get; set; }
        public Faktura[] Fakturor { get; set; }

        public int? ÄrendeNummer { get; set; }
    }
}
