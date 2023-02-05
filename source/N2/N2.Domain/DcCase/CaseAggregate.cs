using N2.Domain.DcCase.Commands;
using N2.Domain.DcCase.Events;
using N2.Model;
using System.Net.Http.Headers;

namespace N2.Domain.DcCase;
public class CaseAggregate : IAggregate<ICaseCommand, ICaseEvent>
{
	private bool _hydrated = false;

	public CaseAggregate(string caseId)
	{
		CaseId = caseId;
	}

	public string Identity => CaseId;
	public ulong Revision { get; set; } = 0UL;

	public string CaseId { get; }
	private string StreamName => this.GetStreamNameForAggregate();


	public DcCaseEntity? Root { get; private set; }
	private DcCaseEntity CheckedRoot => Root ?? throw new AggregateRootIsNullException();

	public CollectionProcess? CollectionProcess { get; private set; }

	public ISet<DebtorCost> Costs { get; } = new HashSet<DebtorCost>();
	public IList<DcCaseNote> Notes { get; } = new List<DcCaseNote>();

	public void Hydrate(IEnumerable<EventReadResult> events)
	{
		foreach (var eventReadResult in events)
		{
			if (eventReadResult.Event is ICaseEvent caseEvent)
			{
				Apply(caseEvent);
				Revision = eventReadResult.EventNumber;
			}
		}

		_hydrated = true;
	}

	public async Task Receive(IEventSender sender, ICaseCommand command)
	{
		if (!_hydrated)
		{
			throw new InvalidOperationException("This aggregate is not hydrated");
		}

		if (command is CreateNewCaseCommand createNewCaseCommand)
		{
			ValidateIsOkForCreate(createNewCaseCommand);
			var @event = new CaseCreated(
				CaseId,
				createNewCaseCommand.ClientIdentity,
				createNewCaseCommand.DebtorIdentities,
				createNewCaseCommand.DebtIdentities,
				createNewCaseCommand.CollectionProcess,
				GeneratePaymentReference());
			var revision = await sender.Send(StreamName, @event);
			Apply(@event);
			Revision = revision;
		}
		else if (command is GenerateNewPaymentReferenceCommand)
		{
			await AcceptGenerateNewPaymentReference(sender);
		}
	}

	private void ValidateIsOkForCreate(CreateNewCaseCommand createNewCaseCommand)
	{
		List<AggregateInvariantViolated> violations = new();
		if (createNewCaseCommand.DebtIdentities?.Any() is false)
		{
			violations.Add(new AggregateInvariantViolated("Missing debts", nameof(CreateNewCaseCommand.DebtIdentities)));
		}
		// etc
		if (violations.Any())
		{
			throw new AggregateInvariantViolationException() { ViolatedInvariants = violations };
		}
	}

	private async Task AcceptGenerateNewPaymentReference(IEventSender sender)
	{
		var newRef = GeneratePaymentReference();
		var @event = new PaymentReferenceGenerated(newRef);
		var revision = await sender.Send(StreamName, @event);
		Apply(@event);
		Revision = revision;
	}

	private void Apply(ICaseEvent @event)
	{
		if (@event is CaseCreated caseCreated)
		{
			Root = new DcCaseEntity(
				CaseId,
				caseCreated.ClientIdentity,
				caseCreated.DebtorIdentities,
				caseCreated.DebtIdentities,
				caseCreated.PaymentReference);
		}
		else if (@event is PaymentReferenceGenerated paymentReferenceGenerated)
		{
			Root = CheckedRoot with { PaymentReference = paymentReferenceGenerated.PaymentReference };
		}
	}

	private static string GeneratePaymentReference()
	{
		var uniqueValue = Guid.NewGuid().ToByteArray();
		var b64 = Convert.ToBase64String(uniqueValue);
		var result = b64
			.Replace("/", "_")
			.Replace("+", ".")
			.Replace("==", string.Empty);
		return result;
	}
}
