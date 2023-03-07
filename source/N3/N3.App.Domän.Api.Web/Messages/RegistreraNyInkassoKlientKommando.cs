using N3.App.Domän.Api.Web.ApiModels;
using N3.CqrsEs.Ramverk;
using SmartAnalyzers.CSharpExtensions.Annotations;

namespace N3.App.Domän.Api.Web.Messages
{
    [InitRequired]
    public record RegistreraNyInkassoKlientKommando(
        string Id,
        RegistreraNyInkassoKlientApiModell Data
    ) : IKommando
    {
        public string Auktorisering => "SYSTEM~";

        public long FörväntadRevision => 0;

        public string KorrelationsIdentifierare { get; init; }

        public IEnumerable<string> Historia { get; } =
            new List<string> { typeof(RegistreraNyInkassoKlientKommando).Assembly.FullName! };
    }
}
