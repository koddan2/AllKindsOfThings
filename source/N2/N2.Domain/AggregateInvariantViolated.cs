namespace N2.Domain
{
	public record AggregateInvariantViolated(string Message, string PropertyName);
}