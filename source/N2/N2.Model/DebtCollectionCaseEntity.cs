using System.ComponentModel.DataAnnotations;

namespace N2.Model
{
	public record DebtCollectionCaseEntity(
		string Identity,
		string ClientIdentity,
		[property: MinLength(1)] ISet<string> DebtorIdentities,
		[property: MinLength(1)] ISet<string> DebtIdentities,
		CollectionProcess CollectionProcess,
		string PaymentReference) : IEntity;
}