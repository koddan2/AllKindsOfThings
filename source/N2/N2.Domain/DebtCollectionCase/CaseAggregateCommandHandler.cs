using N2.Domain.DebtCollectionCase.Commands;

namespace N2.Domain.DebtCollectionCase;

public class CaseAggregateCommandHandler : ICommandHandler<CaseAggregate, ICaseCommand>
{
	private readonly IEventReader _reader;
	private readonly IEventSender _sender;

	public CaseAggregateCommandHandler(IEventReader reader, IEventSender sender)
	{
		_reader = reader;
		_sender = sender;
	}

	public async Task Hydrate(CaseAggregate aggregate, ExpectedStateOfStream expectedState = ExpectedStateOfStream.Exist)
	{
		IEnumerable<EventReadResult> events;
		if (aggregate.Revision > 0)
		{
			events = await _reader.ReadFrom(
			   aggregate.GetStreamNameForAggregate(),
			   aggregate.Revision);
		}
		else
		{
			events = await _reader.Read(
			   aggregate.GetStreamNameForAggregate(),
			   expectedState);
		}
		aggregate.Hydrate(events);
	}
	public async Task Handle(CaseAggregate aggregate, ICaseCommand command)
	{
		await Hydrate(aggregate, command is CreateNewCaseCommand ? ExpectedStateOfStream.Absent : ExpectedStateOfStream.Exist);
		await aggregate.Receive(_sender, command);
	}
}
