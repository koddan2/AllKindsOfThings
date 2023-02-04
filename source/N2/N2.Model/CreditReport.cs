namespace N2.Model
{
	public readonly record struct CreditReport(DateTimeOffset RegisteredAt, byte[] Data);
}