using N2.Domain;

namespace N2.Domain.Services
{
	public interface IEventSender
	{
		Task<ulong> Send(string streamName, IEvent @event);
	}
}