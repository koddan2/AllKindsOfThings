using Ncs.Domain.Model;
using System.Diagnostics;

namespace Ncs.EventSourcing
{
	public record AggregateStream(string Name, string Id);
	public record AggregateStreamCategory(string Name);
	public static class DomainExtensions
	{
		public static string FormatStreamName(this IDomainEvent @event)
		{
			return $"{@event.AggregateName}-{@event.Id}";
		}

		public static string FormatStreamName(this AggregateStream metadata)
		{
			return $"{metadata.Name}-{metadata.Id}";
		}

		public static string FormatStreamName(this AggregateStreamCategory metadata)
		{
			return $"$ce-{metadata.Name}";
		}
	}
}