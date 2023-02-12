namespace N3.SkrivModell.Kommando
{
	public interface IÄrendeNummerUträknare
	{
		Task<long> TaFramNästaLedigaÄrendeNummer();
	}
}
