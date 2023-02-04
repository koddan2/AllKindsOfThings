namespace N2.Domain.Test
{
	internal class TestEventReader : IEventReader
	{
		private readonly TestEventLog _eventLog;

		public TestEventReader(TestEventLog eventLog)
		{
			_eventLog = eventLog;
		}

		async Task<IEnumerable<IEvent>> IEventReader.Read(string id)
		{
			await Task.CompletedTask;
			return _eventLog.Database[id].ToList();
		}
	}
}