namespace N2.Model
{
	public readonly record struct Country
	{
		public static Country Sweden => new("SE");

		private Country(string ISO_3166_2_CountryCode)
		{
			this.ISO_3166_2_CountryCode = ISO_3166_2_CountryCode;
		}

		public string ISO_3166_2_CountryCode { get; }
	}
}