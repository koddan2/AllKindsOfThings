using SmartAnalyzers.CSharpExtensions.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Ncs.Model.Database.Abstractions
{
    [InitRequired]
    public abstract class BaseTransactionalDatabaseModelWithIdentifiers : BaseTransactionalDatabaseModel
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public Guid UniqueId { get; set; }
    }
}
