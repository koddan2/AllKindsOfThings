using SAK;
using System.Text.Json;

namespace Ncs.Domain.Model
{
	public class DebtCollectionClientAggregate
	{
		public static readonly string AggregateName = "DebtCollectionClient";

		public DebtCollectionClientAggregate(string id)
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
	}
}
