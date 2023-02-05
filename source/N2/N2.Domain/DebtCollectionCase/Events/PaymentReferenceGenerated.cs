using N2.EventSourcing.Common;

namespace N2.Domain.DebtCollectionCase.Events;

[N2Event]
public readonly record struct PaymentReferenceGenerated(string PaymentReference) : ICaseEvent;