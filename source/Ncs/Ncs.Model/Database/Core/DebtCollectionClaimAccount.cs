using SmartAnalyzers.CSharpExtensions.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ncs.Model.Database.Core
{
    /// <summary>
    /// SE: inkassoärende
    /// </summary>
    [Table("claimgroup")]
    [InitRequired]
    public class DebtCollectionClaimAccount : BaseTransactionalDatabaseModelWithIdentifiers
    {
        public string ReferenceCode { get; set; }

        [ForeignKey(nameof(ClaimantPartyId))]
        public virtual DebtCollectionParty ClaimantParty { get; set; }
        public long ClaimantPartyId { get; set; }

        public virtual ICollection<DebtCollectionClaim> Claims { get; set; }
    }
}
