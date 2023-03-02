using N3.CqrsEs.Ramverk;
using N3.Modell;

namespace N3.CqrsEs.LäsModell.Frågor
{
    public record HämtaSpecifiktInkassoÄrende(
        string ÄrendeIdentifierare,
        string KorrelationsIdentifierare
    ) : IMeddelande
    {
        public IEnumerable<string> Historia { get; } = new List<string>();
    }
}
