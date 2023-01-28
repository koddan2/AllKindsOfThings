using SmartAnalyzers.CSharpExtensions.Annotations;

namespace Ncs.Model.Database.Core
{
    [InitRequired]
    public abstract class BaseTransactionalDatabaseModel
    {
        public DateTimeOffset CreatedAt { get; set; }

        public string CreatedByUserIdentifier { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }

        public string? UpdatedByUserIdentifier { get; set; }

        public bool SoftDeleted { get; set; } = false;

        public DateTimeOffset? SoftDeletedAt { get; set; }
    }
}
