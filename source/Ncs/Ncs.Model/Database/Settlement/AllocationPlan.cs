using SmartAnalyzers.CSharpExtensions.Annotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ncs.Model.Database.Settlement
{
    [Table("allocation_plan")]
    [InitRequired]
    public class AllocationPlan
    {
        [Required]
        public string Name { get; set; }
    }
    [Table("allocation_plan_entry")]
    [InitRequired]
    public class AllocationPlanEntry
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public short PriorityOrder { get; set; }

        [ForeignKey(nameof(AllocationPlanId))]
        public virtual AllocationPlan AllocationPlan { get; set; }
        [Required]
        public long AllocationPlanId { get; set; }
    }
}
