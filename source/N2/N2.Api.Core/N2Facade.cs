using N2.Domain.DebtCollectionCase;
using N2.Domain.DebtCollectionCase.Commands;

namespace N2.Api.Core;

public record DcCaseViewModel(string Identity, string PaymentReference);
public record CreateDcCaseViewModel(string Identity, string ClientIdentity);
public class N2Facade
{
	private readonly CaseAggregateCommandHandler _caseAggregateCommandHandler;
	public N2Facade(CaseAggregateCommandHandler caseAggregateCommandHandler)
	{
		_caseAggregateCommandHandler = caseAggregateCommandHandler;
	}

	public async Task<CaseAggregate> GetSingleDcCase(string caseIdentity)
	{
		var aggregate = new CaseAggregate(caseIdentity);
		await _caseAggregateCommandHandler.Hydrate(aggregate);
		return aggregate;
	}

	public async Task CreateDcCase(CreateDcCaseViewModel model)
	{
		var aggregate = new CaseAggregate(model.Identity);
		var command = new CreateNewCaseCommand
		{
			ClientIdentity = model.ClientIdentity,
		};
		await _caseAggregateCommandHandler.Handle(aggregate, command);
	}
}