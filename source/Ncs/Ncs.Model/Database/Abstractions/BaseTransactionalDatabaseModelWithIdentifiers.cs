using SmartAnalyzers.CSharpExtensions.Annotations;

namespace Ncs.Model.Database.Core
{
    [InitRequired]
    public abstract class BaseTransactionalDatabaseModelWithIdentifiers : BaseTransactionalDatabaseModel
    {
        public Guid UniqueId { get; set; }

        public long Id { get; set; }
    }
}
