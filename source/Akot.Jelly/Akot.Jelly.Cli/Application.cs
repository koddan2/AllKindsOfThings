using Akot.Jelly.Lib.EventSourcing;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using SqlKata;
using SqlKata.Execution;
using System.Data;
using System.Globalization;

internal class Application
{
    interface IStream<TEventBase> { }
    abstract record Stream<TEventBase>(Guid Id) : IStream<TEventBase>
    {
        protected void Replay(IEnumerable<TEventBase> events)
        {

        }
    }
    interface IPersonEvent { }

    record Person : Stream<IPersonEvent>
    {
        public string? Name { get; set; }
        public string? FavoriteColor { get; set; }

        public Person(Guid Id, IEnumerable<IPersonEvent> events)
            : base(Id)
        {
            this.Replay(events);
        }
    }

    record InitializePerson(string Name, string FavoriteColor);

    record NameChange(string Name);

    record PersonInitializedEvent : Event<InitializePerson>, IPersonEvent
    {
        public PersonInitializedEvent(Guid Id, Guid StreamId, uint SequenceNumber, InitializePerson Payload)
            : base(Id, StreamId, SequenceNumber, Payload)
        {
        }
    }

    record NameChangedEvent : Event<NameChange>, IPersonEvent
    {
        public NameChangedEvent(Guid Id, Guid StreamId, uint SequenceNumber, NameChange Payload)
            : base(Id, StreamId, SequenceNumber, Payload)
        {
        }
    }

    private readonly QueryFactory db;

    public Application(QueryFactory db)
    {
        this.db = db;
    }

    private Query Events => db.Query("application_event");
    private Query SingleStream(Guid streamId) => Events.Where("stream_id", streamId.ToString("D"));

    internal void Run()
    {
        var streamId = Guid.Parse("53c1769c-c127-4c24-9334-efeb4ac6ff15");
        Update(streamId, (id, sn) =>
            new PersonInitializedEvent(id, streamId, sn, new InitializePerson("Adam", "Red")));
        Update(streamId, (id, sn) =>
            new NameChangedEvent(id, streamId, sn, new NameChange("Barney")));
    }

    private void Update<T>(Guid streamId, Func<Guid, uint, Event<T>> maker)
    {
        var dbTransaction = db.Connection.BeginTransaction();
        var maxSequenceNumber = SingleStream(streamId).Max<uint?>("sequence_number", dbTransaction);
        StoreEvent(maker(Guid.NewGuid(), (maxSequenceNumber ?? 0) + 1));
        dbTransaction.Commit();
    }

    void StoreEvent<T>(Event<T> @event, IDbTransaction? dbTransaction = default)
    {
        Events.Insert(new
        {
            id = @event.Id.ToString("D"),
            stream_id = @event.StreamId.ToString("D"),
            sequence_number = @event.SequenceNumber,
            event_type = @event.EventType,
            payload = @event.SerializedPayload,
            created_at = DateTimeOffset.Now.ToString("O"),
        }, dbTransaction);
    }
}