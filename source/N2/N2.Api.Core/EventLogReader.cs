using N2.Domain;
using N2.Domain.DcCase.Events;
using System.Diagnostics;

namespace N2.Api.Core;

public abstract class EventLogReader<TAggregate, TCommand, TEvent>
	where TAggregate : IAggregate<TCommand, TEvent>
{
	private readonly IEventReader _eventReader;

	public EventLogReader(IEventReader eventReader)
	{
		_eventReader = eventReader;
	}

	protected abstract TAggregate NewAggregate(string identity);

	public async Task<IEnumerable<EventReadResult>> ReadFrom(string identity, ulong position)
	{
		return await _eventReader.ReadFromPosition(NewAggregate(identity).GetStreamNameForAggregate(), position);
	}

	public IAsyncEnumerable<EventReadResult> ReadAllEvents(string eventType, ulong position, ulong count)
	{
#if DEBUG
		Debug.Assert(new[] {
			nameof(CaseCreated),
			nameof(PaymentReferenceGenerated),
			// etc.
		}.Contains(eventType), "Must be of correct event type");
#endif
		return _eventReader.ReadAllEventsOfType(eventType, position, count);
	}
}
