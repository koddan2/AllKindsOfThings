
using N3.Modell;

namespace N3.CqrsEs.SkrivModell.Anhopning
{
    public record ÄrendeImportModell(
        UnikIdentifierare ÄrendeIdentifierare,
        UnikIdentifierare KlientReferens,
        GäldenärsInfo[] GäldenärsInfo,
        Faktura[] Fakturor,
        SkuldElement[] FlerSkuldElement
        );
}
