using N2.Domain;

namespace N2.Domain.Services;
public interface IEventReader
{
	Task<IEnumerable<EventReadResult>> Read(string streamName, ExpectedStateOfStream expectedState = ExpectedStateOfStream.Any);

	//
	// Implies that the caller expects the stream to exist.
	Task<IEnumerable<EventReadResult>> ReadFromPosition(string streamName, ulong position);

	//
	// Read all events of a certain type.
	IAsyncEnumerable<EventReadResult> ReadAllEventsOfType(string eventType, ulong position = 0, ulong count = ulong.MaxValue);
}