using N3.CqrsEs.Ramverk;
using SmartAnalyzers.CSharpExtensions.Annotations;
using System.ComponentModel.DataAnnotations;

namespace N3.CqrsEs.SkrivModell.Kommando
{
    [InitRequired]
    public sealed class SkapaEllerUppdateraInkassoGäldenärKommando : IKommando, IValidatableObject
    {
        public string Auktorisering { get; init; }
        public long FörväntadRevision { get; init; }

        [Required]
        public string KorrelationsIdentifierare { get; init; }
        public IEnumerable<string> Historia { get; } = new List<string>();
        public string AggregatIdentifierare { get; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            foreach (var result in this.ValideraKommando())
            {
                yield return result;
            }
        }
    }
}
