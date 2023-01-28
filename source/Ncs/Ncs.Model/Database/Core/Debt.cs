namespace Ncs.Model.Database.Core
{
    public enum DebtorRelationType
    {
        Unknown = 0,
        Main,
        Solidary,
        Guarantor,
    }
    public class Debt
    {
        public long ClaimantPartyId { get; set; }
        public IDictionary<long, DebtorRelationType> Debtors { get; set; } = new Dictionary<long, DebtorRelationType>();
    }
}
