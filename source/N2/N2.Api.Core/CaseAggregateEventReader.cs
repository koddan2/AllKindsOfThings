using N2.Domain.DcCase;
using N2.Domain.DcCase.Commands;
using N2.Domain.DcCase.Events;
using N2.Domain.Services;

namespace N2.Api.Core;

public class CaseAggregateEventReader : EventLogReader<CaseAggregate, ICaseCommand, ICaseEvent>
{
	public CaseAggregateEventReader(IEventReader eventReader) : base(eventReader) { }
	protected override CaseAggregate NewAggregate(string identity) => new(identity);
}
