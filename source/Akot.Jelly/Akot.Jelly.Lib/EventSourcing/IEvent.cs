using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akot.Jelly.Lib.EventSourcing
{
    public interface IEvent<TPayload>
    {
        public Guid Id { get; }

        public Guid StreamId { get; }

        public uint SequenceNumber { get; }

        public string EventType { get; }

        public TPayload Payload { get; }
    }

    public abstract record Event<TPayload>(Guid Id, Guid StreamId, uint SequenceNumber, TPayload Payload) : IEvent<TPayload>
    {
        public virtual string EventType => this.GetType().Name;

        public virtual string SerializedPayload => JsonConvert.SerializeObject(this.Payload, Formatting.Indented);
    }
}
