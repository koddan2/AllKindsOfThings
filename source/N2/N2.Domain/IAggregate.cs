namespace N2.Domain
{
	public interface IAggregate<TCommand, TEvent>
	{
		string Identity { get; }
		ulong Revision { get; }

		void Hydrate(IEnumerable<EventReadResult> events);

		Task Handle(IEventSender sender, TCommand command);
	}
}