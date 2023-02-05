namespace N2.EventSourcing
{
	public record N2Registry(IDictionary<string, Type> Events);
}