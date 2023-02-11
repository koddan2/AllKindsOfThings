namespace N3.Model
{
	/// <summary>
	/// Ett globalt unikt värde, bestående av alfanumeriska tecken
	/// som är shiftlägeskänsliga.
	/// Förkortat "UID".
	/// Exempel:
	/// 4QDKqvHAPEldy3ijc1HX95
	/// xBIimss7CkAszsCokUzHv
	/// 3hGEwuogUUNYKawTv3eRHj
	/// AE8Ls2SXOKGBvKdtJ8fCT
	/// Tekniskt: En System.Guid uttryckt i Bas-62
	/// bibliotek: https://www.nuget.org/packages/Base62-Net
	/// </summary>
	/// <param name="Värde">Själva värdet.</param>
	public readonly record struct UnikIdentifierare(string Värde)
	{
		public static implicit operator string(UnikIdentifierare u) => u.Värde;
		public static implicit operator UnikIdentifierare(string s) => new(s);
	}
}