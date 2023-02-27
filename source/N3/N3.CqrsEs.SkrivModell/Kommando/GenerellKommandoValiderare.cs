using N3.CqrsEs.Ramverk;
using System.ComponentModel.DataAnnotations;

namespace N3.CqrsEs.SkrivModell.Kommando
{
    public static class GenerellKommandoValiderare
    {
        public static IEnumerable<ValidationResult> ValideraKommando(
            this IKommando kommando
        ////ValidationContext validationContext
        )
        {
            if (kommando.FörväntadRevision is 0)
            {
                yield return new ValidationResult(
                    "Saknar värde",
                    new[] { nameof(kommando.FörväntadRevision) }
                );
            }
            if (string.IsNullOrEmpty(kommando.Auktorisering))
            {
                yield return new ValidationResult(
                    "Saknar värde",
                    new[] { nameof(kommando.Auktorisering) }
                );
            }
        }
    }
}
