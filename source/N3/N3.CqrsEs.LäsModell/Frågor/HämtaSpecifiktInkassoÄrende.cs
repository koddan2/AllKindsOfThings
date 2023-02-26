using N3.CqrsEs.Ramverk;
using N3.Modell;

namespace N3.CqrsEs.LäsModell.Frågor
{
    public record HämtaSpecifiktInkassoÄrende(
        UnikIdentifierare ÄrendeIdentifierare,
        UnikIdentifierare KorrelationsIdentifierare
    ) : IMeddelande
    {
        public IEnumerable<string>? Historia { get; set; } = new List<string>();
    }
}
