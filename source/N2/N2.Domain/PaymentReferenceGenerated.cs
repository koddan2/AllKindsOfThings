using N2.Model;
using System.ComponentModel.DataAnnotations;

namespace N2.Domain
{
	public readonly record struct CaseCreated(
		string Identity,
		string ClientIdentity,
		[property: MinLength(1)] ISet<string> DebtorIdentities,
		[property: MinLength(1)] ISet<string> DebtIdentities,
		CollectionProcess CollectionProcess,
		string PaymentReference) : ICaseEvent;
	public readonly record struct PaymentReferenceGenerated(string PaymentReference) : ICaseEvent;
}