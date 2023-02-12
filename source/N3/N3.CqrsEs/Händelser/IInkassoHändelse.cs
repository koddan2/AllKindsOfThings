using CQRSlite.Events;

namespace N3.CqrsEs.Händelser
{
    public interface IInkassoHändelse : IEvent
    {
        string AggregatNamn { get; }
    }
}
