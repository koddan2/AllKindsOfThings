namespace N2.Domain;

public readonly record struct EventMetadata(string EventName)
{
	public bool Validate(IEvent @event)
		=> @event.GetType().Name == EventName;
}
public static class EventExtensions
{
	public static string GetStreamNameForEvent(this EventMetadata eventMetadata)
	{
		return $"$et-{eventMetadata.EventName}";
	}
}

public static class AggregateExtensions
{
	public static string GetStreamNameForAggregate<TCommand, TEvent>(this IAggregate<TCommand, TEvent> aggregate)
		where TEvent : IEvent
		=> $"{aggregate.GetType().Name}-{aggregate.Identity}";

	public static string GetStreamNameForAggregateCategory<TCommand, TEvent>(this IAggregate<TCommand, TEvent> aggregate)
		where TEvent : IEvent
		=> $"$ce-{aggregate.GetType().Name}";
}