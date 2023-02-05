using N2.Domain;
using N2.Domain.DcCase;
using N2.Domain.DcCase.Commands;

namespace N2.Api.Core;

public record DcCaseViewModelCreate(string Identity, string ClientIdentity);
public class N2Facade
{
	private readonly CaseAggregateEventReader _caseAggregateEventReader;
	private readonly CaseAggregateCommandHandler _caseAggregateCommandHandler;

	public N2Facade(
		CaseAggregateEventReader caseAggregateEventReader,
		CaseAggregateCommandHandler caseAggregateCommandHandler)
	{
		_caseAggregateEventReader = caseAggregateEventReader;
		_caseAggregateCommandHandler = caseAggregateCommandHandler;
	}

	public async Task<IEnumerable<EventReadResult>> DcCaseGetLogSingle(string caseIdentity, ulong position = 0UL)
	{
		return await _caseAggregateEventReader.ReadFrom(caseIdentity, position);
	}

	public async Task<IEnumerable<EventReadResult>> DcCaseGetLogOf(string eventType, ulong skip, ulong take)
	{
		var result = await _caseAggregateEventReader.ReadAllEvents(eventType, skip, take).ToListAsync();
		return result;
	}

	public async Task DcCaseCreate(DcCaseViewModelCreate model)
	{
		var aggregate = new CaseAggregate(model.Identity);
		var command = new CreateNewCaseCommand
		{
			ClientIdentity = model.ClientIdentity,
		};
		await _caseAggregateCommandHandler.Handle(aggregate, command);
	}
}