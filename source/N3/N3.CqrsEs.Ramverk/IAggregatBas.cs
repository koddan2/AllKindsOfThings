using System.Text.Json.Serialization;

namespace N3.CqrsEs.Ramverk
{
    /// <summary>
    /// Dvs. AggregateRoot
    /// </summary>
    public interface IAggregatBas
    {
        string Id { get; }
        UnikIdentifierare Identifierare { get; }
        int Version { get; }
    }

    // Infrastructure to capture modifications to state in events
    public abstract class AbstraktAggregatBasKlass<T> : IAggregatBas
        where T : IAggregatBas
    {
        ////public UnikIdentifierare Identifierare { get; private set; }

        public AbstraktAggregatBasKlass() { }
            ////=>
            ////Identifierare = UnikIdentifierare.Ingen;
        public AbstraktAggregatBasKlass(UnikIdentifierare identifierare) =>
            Identifierare = identifierare;

        // For indexing our event streams
        public string Id { get; set; }
        ////{
        ////    get
        ////    {
        ////        return new AggregatStrömIdentifierare(typeof(T), Identifierare).ByggStrömIdentifierare();
        ////    }
        ////    set
        ////    {
        ////        this.Identifierare = new AggregatIdentifierareFrånStröm(typeof(T), value).TillIdentifierare();
        ////    }
        ////}

        // For protecting the state, i.e. conflict prevention
        public int Version { get; protected set; }

        // JsonIgnore - for making sure that it won't be stored in inline projection
        [JsonIgnore]
        private readonly List<object> _uncommittedEvents = new();

        // Get the deltas, i.e. events that make up the state, not yet persisted
        public IEnumerable<object> GetUncommittedEvents()
        {
            return _uncommittedEvents;
        }

        // Mark the deltas as persisted.
        public void ClearUncommittedEvents()
        {
            _uncommittedEvents.Clear();
        }

        protected void AddUncommittedEvent(object @event)
        {
            // add the event to the uncommitted list
            _uncommittedEvents.Add(@event);
        }
    }
}
