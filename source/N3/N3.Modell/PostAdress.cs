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
        string Ortsnamn,
        Land Land
    )
    {
        private static readonly Regex _Whitespace = GenerellaRegexUttryck.AllaBlankTecken();
        public int NumerisktPostNummer => int.Parse(_Whitespace.Replace(PostNummer, string.Empty));
    }
}
