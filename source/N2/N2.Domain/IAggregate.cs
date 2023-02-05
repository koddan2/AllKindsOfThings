namespace N2.Domain
{
	public interface IAggregate<TCommand, TEvent>
		where TEvent : IEvent
	{
		string Identity { get; }
		ulong Revision { get; }

		internal void Hydrate(IEnumerable<EventReadResult> events);

		public Task ReceiveCommand(Func<TEvent, Task<ulong>> send, TCommand command);
	}
}