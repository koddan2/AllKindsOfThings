namespace N2.Model
{
	public record DcClientEntity(string Identity) : IEntity
	{
		public string? Name { get; private set; }
		public string? NationalIdentification { get; private set; }
		public ISet<PostalAddress> PostalAddresses { get; } = new HashSet<PostalAddress>();
		public ISet<CollectionProcess> CollectionProcesses { get; } = new HashSet<CollectionProcess>();
	}
}