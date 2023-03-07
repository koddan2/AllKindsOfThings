using SmartAnalyzers.CSharpExtensions.Annotations;
using System.ComponentModel.DataAnnotations;

namespace N3.App.Domän.Api.Web.ApiModels
{
    public record Receipt(string Id);

    public class UnikIdentifierareAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is UnikIdentifierare)
            {
                return true;
            }
            else if (value is string strVal)
            {
                if (UnikIdentifierare.ÄrGodkänd(strVal))
                {
                    return true;
                }
                else if (Guid.TryParse(strVal, out _))
                {
                    return true;
                }
            }
            else
            {
                return true;
            }

            return false;
        }
    }

    [InitRequired]
    public class PostAdress
    {
        [Required]
        public string AdressRad1 { get; init; }

        [Required]
        public string PostNummer { get; init; }
    }
}
