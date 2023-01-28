using Ncs.Model.Database.Abstractions;
using SmartAnalyzers.CSharpExtensions.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ncs.Model.Database.Core.Parties
{
    [InitRequired]
    public class DebtCollectionDebtor : BaseTransactionalDatabaseModelWithIdentifiers
    {
        [ForeignKey(nameof(PartyId))]
        public virtual DebtCollectionParty Party { get; set; }
        public long PartyId { get; set; }
    }
}
