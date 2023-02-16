using N3.CqrsEs.LäsModell.DataÖverföring;
using N3.Modell;

namespace N3.CqrsEs.LäsModell.Frågor
{
    public interface IQueryProcessor
    {
        Task<InkassoÄrendeFullVyModell> Hantera(HämtaSpecifiktInkassoÄrende fråga);
    }
	public record HämtaSpecifiktInkassoÄrende(UnikIdentifierare ÄrendeIdentifierare);
}
