using Ncs.Model.Database.Abstractions;
using SmartAnalyzers.CSharpExtensions.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ncs.Model.Database.Core.Parties
{
    [Table("party", Schema = Schemas.CoreDebtCollection)]
    [InitRequired]
    public class DebtCollectionParty : BaseTransactionalDatabaseModelWithIdentifiers
    {

    }
}
