using SmartAnalyzers.CSharpExtensions.Annotations;
using System.ComponentModel.DataAnnotations;

namespace N3.App.Domän.Api.Web.ApiModels
{
    [InitRequired]
    public class RegistreraNyInkassoKlientApiModell
    {
        [UnikIdentifierare]
        public string? Id { get; init; }

        [Required]
        public string Namn { get; init; }

        //[Required]
        //[MinLength(1)]
        //public PostAdress[] Adresser { get; init; }
    }
}
