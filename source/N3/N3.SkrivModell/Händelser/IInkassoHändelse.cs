using CQRSlite.Events;

namespace N3.SkrivModell.Händelser
{
	public interface IInkassoHändelse : IEvent
	{
		string AggregateName { get; }
	}
}
