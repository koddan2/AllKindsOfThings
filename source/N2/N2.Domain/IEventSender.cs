namespace N2.Domain
{
	public interface IEventSender
	{
		Task Send(string streamName, IEvent @event);
	}
}