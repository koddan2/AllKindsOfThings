namespace Ncs.Domain.Model
{
	public record DebtCollectionClientDeletedEvent_V1(string Id) : IDomainEvent
	{
		public uint Version => 1;
		public string EventName => nameof(DebtCollectionClientDeletedEvent_V1);
		public string AggregateName => DebtCollectionClientAggregate.AggregateName;
	}
}
