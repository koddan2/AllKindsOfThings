namespace N2.EventSourcing
{
	public record N2Registry(IReadOnlyDictionary<string, Type> Events);
}