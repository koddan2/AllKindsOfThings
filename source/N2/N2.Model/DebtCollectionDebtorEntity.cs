namespace N2.Model
{
	public record DebtCollectionDebtorEntity(string Identity, string Name) : IEntity
	{
		public string? NationalIdentification { get; private set; }
		public DebtCollectionDebtorBaseType? BaseType { get; private set; }
		public DebtCollectionDebtorJuridicalType? JuridicalType { get; private set; }
		public ISet<PostalAddress> PostalAddresses { get; } = new HashSet<PostalAddress>();
		public ISet<CreditReport> CreditReports { get; } = new HashSet<CreditReport>();
	}
}