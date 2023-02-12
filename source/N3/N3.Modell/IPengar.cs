namespace N3.Modell
{
	////public readonly record struct Pengar(decimal Belopp, string ValutaKod = "") : IPengar;
	/// <summary>
	/// Motsvarar ett belopp av pengar i någon valuta
	/// </summary>
	public interface IPengar
	{
		decimal Belopp { get; }
		string ValutaKod { get; }
	}
}