using CQRSlite.Queries;
using N3.CqrsEs.LäsModell.DataÖverföring;
using N3.CqrsEs.LäsModell.HändelseHantering;
using N3.Modell;
using SmartAnalyzers.CSharpExtensions.Annotations;

namespace N3.CqrsEs.LäsModell.Frågor
{
	public record HämtaSpecifiktInkassoÄrende(UnikIdentifierare ÄrendeIdentifierare) : IQuery<InkassoÄrendeFullVyModell>;
}
