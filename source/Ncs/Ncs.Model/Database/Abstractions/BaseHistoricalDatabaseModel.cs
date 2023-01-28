using SmartAnalyzers.CSharpExtensions.Annotations;

namespace Ncs.Model.Database.Abstractions
{
    [InitRequired]
    public abstract class BaseHistoricalDatabaseModel
    {
        public DateTimeOffset CreatedAt { get; set; }

        public string CreatedByUserIdentifier { get; set; }
    }
}
