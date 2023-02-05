namespace N2.Domain
{
	public interface IEventSender
	{
		Task<ulong> Send(string streamName, IEvent @event);
	}
}