namespace N2.Domain.Test
{
	internal class TestEventSender : IEventSender
	{
		private readonly TestEventLog _eventLog;

		public TestEventSender(TestEventLog eventLog)
		{
			_eventLog = eventLog;
		}

		async Task IEventSender.Send(string streamName, IEvent @event)
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
		}
	}
}