namespace N2.Domain
{
	public interface IAggregate<TCommand, TEvent>
	{
		string Identity { get; }
		ulong Revision { get; }

		internal void Hydrate(IEnumerable<EventReadResult> events);

		internal Task Receive(IEventSender sender, TCommand command);
	}
}