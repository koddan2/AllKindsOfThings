using SAK;
using System.Text.Json;

namespace Ncs.Domain.Model
{
	public class DebtCollectionClientEntity : IAggregate
	{
		public static readonly string EntityName = "DebtCollectionClient";

		public DebtCollectionClientEntity(string id)
		{
			Id = id;
		}

		public string Id { get; }

		public string? Name { get; set; }
		public string? PersonalIdentificationNumber { get; set; }

		public void Apply(string eventName, string eventData)
		{
			if (eventName == nameof(DebtCollectionClientCreatedEvent_V1))
			{
				var data = JsonSerializer
					.Deserialize<DebtCollectionClientCreatedEvent_V1>(eventData)
					.OrFail();
				Name = data.Name;
				PersonalIdentificationNumber = data.PersonalIdentificationNumber;
			}
		}

		public bool CanApply(ICommand command)
		{
			if (command is CreateDebtCollectionCommand)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
