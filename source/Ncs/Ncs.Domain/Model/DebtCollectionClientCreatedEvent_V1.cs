namespace Ncs.Domain.Model
{
	public record DebtCollectionClientCreatedEvent_V1(string Id, DebtCollectionClientCreateModel_V1 Payload) : IDomainEvent
	{
		public uint Version => 1;
	};
}
