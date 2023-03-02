using N3.CqrsEs.Ramverk;
using N3.Modell;
using SmartAnalyzers.CSharpExtensions.Annotations;
using System.ComponentModel.DataAnnotations;

namespace N3.CqrsEs.SkrivModell.Kommando
{
    [InitRequired]
    public sealed class SkapaInkassoÄrendeKommando : IKommando, IValidatableObject
    {
        public SkapaInkassoÄrendeKommando(
            string aggregatIdentifierare,
            string klientReferens,
            string[] gäldenärsReferenser,
            Faktura[] fakturor
        )
        {
            AggregatIdentifierare = aggregatIdentifierare;
            KlientReferens = klientReferens;
            GäldenärsReferenser = gäldenärsReferenser;
            Fakturor = fakturor;
        }

        [Required]
        public string KorrelationsIdentifierare { get; init; }
        public IEnumerable<string> Historia { get; } = new List<string>();

        public string Auktorisering { get; init; }
        public long FörväntadRevision { get; init; }

        [Required]
        public string AggregatIdentifierare { get; }

        [Required]
        public string KlientReferens { get; }

        [Required]
        public string[] GäldenärsReferenser { get; }

        [Required]
        public Faktura[] Fakturor { get; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            foreach (var result in this.ValideraKommando())
            {
                yield return result;
            }

            if (AggregatIdentifierare == UnikIdentifierare.Ingen)
            {
                yield return new ValidationResult(
                    "Saknar värde",
                    new[] { nameof(AggregatIdentifierare) }
                );
            }

            if (KlientReferens == UnikIdentifierare.Ingen)
            {
                yield return new ValidationResult("Saknar värde", new[] { nameof(KlientReferens) });
            }

            if (GäldenärsReferenser?.Length is 0)
            {
                yield return new ValidationResult(
                    "Saknar innehåll",
                    new[] { nameof(GäldenärsReferenser) }
                );
            }

            if (Fakturor?.Length is 0)
            {
                yield return new ValidationResult("Saknar innehåll", new[] { nameof(Fakturor) });
            }

            if (KorrelationsIdentifierare == UnikIdentifierare.Ingen)
            {
                yield return new ValidationResult(
                    "Saknar värde",
                    new[] { nameof(KorrelationsIdentifierare) }
                );
            }
        }
    }
}
