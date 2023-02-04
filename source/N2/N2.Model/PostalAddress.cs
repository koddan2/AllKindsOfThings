namespace N2.Model
{
	public readonly record struct PostalAddress(Country Country, string Line1, string Line2, string Line3, string PostalCode);
}