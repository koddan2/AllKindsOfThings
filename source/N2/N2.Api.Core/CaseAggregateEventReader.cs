using N2.Domain;
using N2.Domain.DcCase;
using N2.Domain.DcCase.Commands;
using N2.Domain.DcCase.Events;

namespace N2.Api.Core;

public class CaseAggregateEventReader : EventLogReader<CaseAggregate, ICaseCommand, ICaseEvent>
{
	public CaseAggregateEventReader(IEventReader eventReader) : base(eventReader) { }
	protected override CaseAggregate NewAggregate(string identity) => new(identity);
}
