﻿using System.Text.Json.Serialization;

namespace Ncs.Domain.Model
{
	public record DebtCollectionClientCreatedEvent_V1(
		string Id,
		string PersonalIdentificationNumber,
		string Name) : IDomainEvent
	{
		public uint Version => 1;
		public string EventName => nameof(DebtCollectionClientCreatedEvent_V1);
		public string AggregateName => DebtCollectionClientAggregate.AggregateName;

		public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;
	};
}
