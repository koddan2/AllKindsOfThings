using N2.Domain.DebtCollectionCase.Commands;
using N2.Domain.DebtCollectionCase.Events;
using N2.Model;

namespace N2.Domain.DebtCollectionCase;

public static class AggregateExtensions
{
	public static string GetStreamNameForAggregate<TCommand, TEvent>(this IAggregate<TCommand, TEvent> aggregate)
		=> $"{aggregate.GetType().Name}-{aggregate.Identity}";
}
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
				caseCreated.CollectionProcess,
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
