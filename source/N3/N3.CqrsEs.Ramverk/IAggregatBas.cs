using SmartAnalyzers.CSharpExtensions.Annotations;
using System.Text.Json.Serialization;

namespace N3.CqrsEs.Ramverk
{
    /// <summary>
    /// Dvs. AggregateRoot
    /// </summary>
    public interface IAggregatBas
    {
        string Id { get; }
        int Version { get; }
    }

    // Infrastructure to capture modifications to state in events
    [InitRequired]
    public abstract class AbstraktAggregatBasKlass : IAggregatBas
    {
        // For indexing our event streams
        public string Id { get; protected set; }

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
