using N3.Model.Hjälpmedel;
using System.Text.RegularExpressions;

namespace N3.Modell
{
	public readonly record struct PostAdress(
		string AdressRad1,
		string? AdressRad2,
		string? AdressRad3,
		string? CareOf,
		string PostNummer,
		Land Land)
	{
		private static readonly Regex Whitespace = GenerellaRegexUttryck.AllaBlankTecken();
		public int NumerisktPostNummer => int.Parse(Whitespace.Replace(PostNummer, string.Empty));
	}
}