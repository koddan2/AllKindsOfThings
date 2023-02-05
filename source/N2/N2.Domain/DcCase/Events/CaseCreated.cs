using N2.EventSourcing.Common;
using N2.Model;
using System.ComponentModel.DataAnnotations;

namespace N2.Domain.DcCase.Events;

[N2Event]
public readonly record struct CaseCreated(
	[Required]string Identity,
	[Required]string ClientIdentity,
	[property: MinLength(1)] ISet<string> DebtorIdentities,
	[property: MinLength(1)] ISet<string> DebtIdentities,
	CollectionProcess? CollectionProcess,
	[Required]string PaymentReference) : ICaseEvent;