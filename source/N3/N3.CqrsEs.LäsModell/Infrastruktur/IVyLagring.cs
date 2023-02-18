using N3.CqrsEs.LäsModell.DataÖverföring;
using N3.CqrsEs.LäsModell.Frågor;

namespace N3.CqrsEs.LäsModell.Infrastruktur
{
	public interface IVyLagring
	{
		Task LäggTillÄrende(InkassoÄrendeFullVyModell inkassoÄrendeFullVyModell, CancellationToken token = default);
		Task<InkassoÄrendeFullVyModell> HämtaSpecifiktÄrende(HämtaSpecifiktInkassoÄrende parametrar, CancellationToken token = default);
	}
}
