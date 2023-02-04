namespace N2.Domain.Test
{
	internal class TestEventLog
	{
		private readonly Dictionary<string, IList<IEvent>> _db = new Dictionary<string, IList<IEvent>>();
		public IDictionary<string, IList<IEvent>> Database => _db;
	}
}