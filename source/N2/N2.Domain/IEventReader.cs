namespace N2.Domain;

public enum ExpectedStateOfStream
{
	Any,
	Exist,
	Absent,
}
public interface IEventReader
{
	Task<IEnumerable<EventReadResult>> Read(string streamName, ExpectedStateOfStream expectedState = ExpectedStateOfStream.Any);
}