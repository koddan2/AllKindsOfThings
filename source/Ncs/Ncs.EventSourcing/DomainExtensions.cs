using Ncs.Domain.Model;
using System.Diagnostics;

namespace Ncs.EventSourcing
{
	public static class DomainExtensions
	{
		public static string FormatStreamName(this IDomainEvent @event)
		{
			return $"{@event.AggregateName}-{@event.Id}";
		}

		public static string FormatStreamName(this (string, string) aggregateNameIdPair, string? maybePrefix = default)
		{
			if (maybePrefix is string prefix)
			{
				Debug.Assert(new[] { "$ce-" }.Contains(prefix), "Must be valid prefix");
				return $"{prefix}{aggregateNameIdPair.Item1}-{aggregateNameIdPair.Item2}";
			}

			return $"{aggregateNameIdPair.Item1}-{aggregateNameIdPair.Item2}";
		}
	}
}