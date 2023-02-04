namespace N2.Domain
{
	public interface IEventReader
	{
		Task<IEnumerable<IEvent>> Read(string streamName);
	}
}