namespace N2.Model
{
	public record DcDebtorEntity(string Identity, string Name) : IEntity
	{
		public string? NationalIdentification { get; private set; }
		public LegalEntityType? BaseType { get; private set; }
		public SwedishCorporateForm? JuridicalType { get; private set; }
		public ISet<PostalAddress> PostalAddresses { get; } = new HashSet<PostalAddress>();
		public ISet<CreditReport> CreditReports { get; } = new HashSet<CreditReport>();
	}
}