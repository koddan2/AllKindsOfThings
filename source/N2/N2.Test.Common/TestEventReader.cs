using N2.Domain;
using N2.Domain.Services;
using N2.EventSourcing.Common;

namespace N2.Test.Common
{
	public class TestEventReader : IEventReader
	{
		private readonly TestEventLog _eventLog;

		public TestEventReader(TestEventLog eventLog)
		{
			_eventLog = eventLog;
		}

		async Task<IEnumerable<EventReadResult>> IEventReader.ReadFromPosition(string streamName, ulong position)
		{
			await ValueTask.CompletedTask;
			return ReadInner(streamName, position);
		}

		async IAsyncEnumerable<EventReadResult> IEventReader.ReadAllEventsOfType(string eventType, ulong position, ulong count)
		{
			await ValueTask.CompletedTask;
			var outerCounter = 0UL;
			foreach (var kvp in _eventLog.Database)
			{
				if (position < outerCounter++)
				{
					continue;
				}
				if (outerCounter >= count)
				{
					break;
				}
				var counter = 0UL;
				foreach (var item in kvp.Value)
				{
					if (item.GetType().Name == eventType)
					{
						yield return new EventReadResult(item, counter++);
					}
				}
			}
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