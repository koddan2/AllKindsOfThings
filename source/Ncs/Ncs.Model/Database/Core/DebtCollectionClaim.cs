using SmartAnalyzers.CSharpExtensions.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ncs.Model.Database.Core
{
    [InitRequired]
    public class DebtCollectionClaim : BaseTransactionalDatabaseModelWithIdentifiers
    {
        [ForeignKey(nameof(DebtCollectionCaseId))]
        public virtual DebtCollectionClaimAccount DebtCollectionCase { get; set; }
        public long DebtCollectionCaseId { get; set; }
    }
}
