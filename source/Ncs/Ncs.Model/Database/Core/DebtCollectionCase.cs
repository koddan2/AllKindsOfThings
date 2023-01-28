using SmartAnalyzers.CSharpExtensions.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ncs.Model.Database.Core
{
    [InitRequired]
    public class DebtCollectionParty : BaseTransactionalDatabaseModelWithIdentifiers
    {

    }
    [InitRequired]
    public class DebtCollectionClient : DebtCollectionParty
    {

    }
    [InitRequired]
    public class DebtCollectionDebtor : DebtCollectionParty
    {

    }
    [InitRequired]
    public class DebtCollectionAgency : DebtCollectionParty
    {

    }
    [InitRequired]
    public class DebtCollectionLegalInstitute : DebtCollectionParty
    {

    }
    [InitRequired]
    public class DebtCollectionLegalItem : BaseTransactionalDatabaseModelWithIdentifiers
    {

    }
    [InitRequired]
    public class DebtCollectionClaim : BaseTransactionalDatabaseModelWithIdentifiers
    {
        [ForeignKey(nameof(DebtCollectionCaseId))]
        public virtual DebtCollectionCase DebtCollectionCase { get; set; }
        public long DebtCollectionCaseId { get; set; }
    }
    [InitRequired]
    public class DebtCollectionCase : BaseTransactionalDatabaseModelWithIdentifiers
    {
        public string ReferenceCode { get; set; }

        [ForeignKey(nameof(CreditorId))]
        public virtual DebtCollectionClient Creditor { get; set; }
        public long CreditorId { get; set; }

        public virtual ICollection<DebtCollectionClaim> Claims { get; set; }
    }
}
