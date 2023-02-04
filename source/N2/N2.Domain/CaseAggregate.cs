using N2.Model;

namespace N2.Domain
{
	public interface IAggregate
	{
		string Identity { get; }
		ulong Revision { get; }
	}
	public class CaseAggregate : IAggregate
	{
		public CaseAggregate(string caseId)
		{
			CaseId = caseId;
		}

		public string Identity => CaseId;
		public ulong Revision { get; set; } = 0UL;

		public string CaseId { get; }
		public string StreamName => $"{nameof(CaseAggregate)}-{CaseId}";

		public DebtCollectionCaseEntity? Root { get; private set; }

		public ISet<DebtorCost> Costs { get; } = new HashSet<DebtorCost>();
		public IList<DebtCollectionCaseNote> Notes { get; } = new List<DebtCollectionCaseNote>();

		public async Task<CaseAggregate> Hydrate(IEventReader reader)
		{
			var events = await reader.Read(StreamName);
			foreach (var @event in events)
			{
				if (@event is ICaseEvent caseEvent)
				{
					Apply(caseEvent);
					// FIXME: well, this should come from IEvent
					Revision += 1;
				}
			}

			return this;
		}

		public async Task Handle(IEventSender sender, ICaseCommand command)
		{
			if (command is CreateNewCaseCommand createNewCaseCommand)
			{
				Root = new DebtCollectionCaseEntity(
					CaseId,
					createNewCaseCommand.ClientIdentity,
					createNewCaseCommand.DebtorIdentities,
					createNewCaseCommand.DebtIdentities,
					createNewCaseCommand.CollectionProcess,
					GeneratePaymentReference());
				var @event = new CaseCreated(
					Root.Identity,
					Root.ClientIdentity,
					Root.DebtorIdentities,
					Root.DebtIdentities,
					Root.CollectionProcess,
					Root.PaymentReference);
				await sender.Send(StreamName, @event);
			}
			else if (command is GenerateNewPaymentReferenceCommand)
			{
				await AcceptGenerateNewPaymentReference(sender);
			}
		}

		private async Task AcceptGenerateNewPaymentReference(IEventSender sender)
		{
			var newRef = GeneratePaymentReference();
			// the following line of code is unnecessary, but added so that VS doesn't complain about the method could be static.
			Root = Root! with { PaymentReference = newRef };
			await sender.Send(StreamName, new PaymentReferenceGenerated(newRef));
		}

		private void Apply(ICaseEvent @event)
		{
			if (@event is CaseCreated caseCreated)
			{
				Root = new DebtCollectionCaseEntity(
					CaseId,
					caseCreated.ClientIdentity,
					caseCreated.DebtorIdentities,
					caseCreated.DebtIdentities,
					caseCreated.CollectionProcess,
					caseCreated.PaymentReference);
			}
			else if (@event is PaymentReferenceGenerated paymentReferenceGenerated)
			{
				Root = Root! with { PaymentReference = paymentReferenceGenerated.PaymentReference };
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
}