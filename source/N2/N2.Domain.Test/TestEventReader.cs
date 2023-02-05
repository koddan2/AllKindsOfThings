using N2.EventSourcing.Common;

namespace N2.Domain.Test
{
	internal class TestEventReader : IEventReader
	{
		private readonly TestEventLog _eventLog;

		public TestEventReader(TestEventLog eventLog)
		{
			_eventLog = eventLog;
		}

		async Task<IEnumerable<EventReadResult>> IEventReader.ReadFrom(string streamName, ulong position)
		{
			await ValueTask.CompletedTask;
			return ReadInner(streamName, position);
		}

		async Task<IEnumerable<EventReadResult>> IEventReader.Read(string streamName, ExpectedStateOfStream expectedState)
		{
			await ValueTask.CompletedTask;
			return expectedState switch
			{
				ExpectedStateOfStream.Any => _eventLog.Database.ContainsKey(streamName)
					? ReadInner(streamName)
					: Array.Empty<EventReadResult>(),
				ExpectedStateOfStream.Exist => _eventLog.Database.ContainsKey(streamName)
					? ReadInner(streamName)
					: throw new ExpectationFailedException($"Expectation was: {expectedState} but stream does not exist."),
				ExpectedStateOfStream.Absent => _eventLog.Database.ContainsKey(streamName)
					? throw new ExpectationFailedException($"Expectation was: {expectedState} but stream exists.")
					: Array.Empty<EventReadResult>(),
			};
		}

		private IEnumerable<EventReadResult> ReadInner(string streamName, ulong position = 0)
		{
			return _eventLog.Database[streamName]
				.Skip((int)position)
				.Select((x, i) => new EventReadResult(x, position + (ulong)i + 1));
		}
	}
}