using System.ComponentModel.DataAnnotations;

namespace Ncs.Domain.Model
{
	public interface IDomainEvent
	{
		[Required]
		string Id { get; }

		[Required]
		uint Version { get; }

		[Required]
		string AggregateName { get; }

		[Required]
		string EventName { get; }
	}
}
