namespace N2.Domain
{
	public readonly record struct EventReadResult(IEvent Event, ulong EventNumber);
}