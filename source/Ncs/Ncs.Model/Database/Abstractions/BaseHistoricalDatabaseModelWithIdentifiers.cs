using SmartAnalyzers.CSharpExtensions.Annotations;

namespace Ncs.Model.Database.Abstractions
{
    [InitRequired]
    public abstract class BaseHistoricalDatabaseModelWithIdentifiers : BaseHistoricalDatabaseModel
    {
        public Guid UniqueId { get; set; }

        public long Id { get; set; }
    }
}
