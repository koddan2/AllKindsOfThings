using N2.Domain;

namespace N2.Test.Common
{
	public class TestEventLog
	{
		private readonly Dictionary<string, IList<IEvent>> _db = new Dictionary<string, IList<IEvent>>();
		public IDictionary<string, IList<IEvent>> Database => _db;
	}
}