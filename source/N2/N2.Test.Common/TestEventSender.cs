using N2.Domain;
using N2.Domain.Services;

namespace N2.Test.Common
{
	public class TestEventSender : IEventSender
	{
		private readonly TestEventLog _eventLog;

		public TestEventSender(TestEventLog eventLog)
		{
			_eventLog = eventLog;
		}

		async Task<ulong> IEventSender.Send(string streamName, IEvent @event)
		{
			await Task.CompletedTask;
			IList<IEvent> eventList;
			if (_eventLog.Database.ContainsKey(streamName))
			{
				eventList = _eventLog.Database[streamName];
			}
			else
			{
				eventList = new List<IEvent>();
				_eventLog.Database[streamName] = eventList;
			}

			eventList.Add(@event);
			return (ulong)eventList.Count;
		}
	}
}