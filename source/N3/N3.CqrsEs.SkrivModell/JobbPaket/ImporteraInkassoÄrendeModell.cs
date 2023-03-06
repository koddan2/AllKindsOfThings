using N3.CqrsEs.Ramverk.Jobs;
using N3.CqrsEs.SkrivModell.Anhopning;
using SmartAnalyzers.CSharpExtensions.Annotations;

namespace N3.CqrsEs.SkrivModell.JobbPaket
{
    [InitRequired]
    public class ImporteraInkassoÄrendeJobbData : AbstractJob
    {
        public ÄrendeImportModell ImportData { get; set; }
    }
}
