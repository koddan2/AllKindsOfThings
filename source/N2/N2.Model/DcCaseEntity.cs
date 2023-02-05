using System.ComponentModel.DataAnnotations;

namespace N2.Model
{
	public record DcCaseEntity(
		string Identity,
		string ClientIdentity,
		[property: MinLength(1)] ISet<string> DebtorIdentities,
		[property: MinLength(1)] ISet<string> DebtIdentities,
		string PaymentReference) : IEntity;
}