using Cqrs.Events;

namespace N3.CqrsEs.Händelser
{
    public interface IInkassoHändelse : IEvent<string>
    {
        string AggregatNamn { get; }
    }
}
