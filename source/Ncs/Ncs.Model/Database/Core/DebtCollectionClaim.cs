using Ncs.Model.Database.Abstractions;
using SmartAnalyzers.CSharpExtensions.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ncs.Model.Database.Core
{
    [Table("claim", Schema = Schemas.CoreDebtCollection)]
    [InitRequired]
    public class DebtCollectionClaim : BaseTransactionalDatabaseModelWithIdentifiers
    {
        public string ClaimNumber { get; set; }
        public string Description { get; set; }

        public DateTimeOffset ClaimDate { get; set; }
        public DateTimeOffset DueDate { get; set; }

        [ForeignKey(nameof(DebtCollectionClaimGroupId))]
        public virtual DebtCollectionClaimGroup DebtCollectionClaimGroup { get; set; }
        public long DebtCollectionClaimGroupId { get; set; }
    }
}
