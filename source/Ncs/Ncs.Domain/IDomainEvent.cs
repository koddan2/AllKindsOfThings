using System.ComponentModel.DataAnnotations;

namespace Ncs.Domain.Model
{
	public interface IAggregate
	{
		string Id { get; }
		bool CanApply(ICommand command);
		void Apply(string eventName, string eventData);
	}
	public interface ICommand { }
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
