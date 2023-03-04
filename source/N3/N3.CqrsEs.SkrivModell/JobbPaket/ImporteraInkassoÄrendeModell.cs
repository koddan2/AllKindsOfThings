using N3.CqrsEs.Ramverk;
using N3.CqrsEs.SkrivModell.Anhopning;
using SmartAnalyzers.CSharpExtensions.Annotations;

namespace N3.CqrsEs.SkrivModell.JobbPaket
{
    [InitRequired]
    public class ImporteraInkassoÄrendeJobb : AbstraktJobb
    {
        public ÄrendeImportModell ImportData { get; set; }
    }
}
