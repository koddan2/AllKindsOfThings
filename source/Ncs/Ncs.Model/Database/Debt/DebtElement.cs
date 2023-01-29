using SmartAnalyzers.CSharpExtensions.Annotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ncs.Model.Database.Debt
{
    public enum DebtElementAmountType
    {
        Principal = 1,
        Interest = 2,
    }

    public enum DebtElementDebtType
    {
        BaseClaim = 1,
        Fee = 2,
    }

    [Table("debt_element", Schema = Schemas.CoreDebtCollection)]
    [InitRequired]
    public class DebtElement
    {
        [Required]
        public string Name { get; set; }
    }
}
