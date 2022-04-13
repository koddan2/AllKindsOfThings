using Akot.Jelly.Lib.EventSourcing;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using SqlKata;
using SqlKata.Execution;
using System.Data;
using System.Globalization;

internal class Application
{
    interface IApplicator<T>
    {
        void Apply(T entity);
    }
    interface IStream<TEntity, TEventBase>
        where TEventBase : IApplicator<TEntity>
    {
    }
    abstract record Stream<TEntity, TEventBase>(Guid Id) : IStream<TEntity, TEventBase>
        where TEventBase : IApplicator<TEntity>
    {
        protected abstract TEntity Entity { get; }
        protected void Replay(IEnumerable<TEventBase> events)
        {
            foreach (var @event in events)
            {
                @event.Apply(Entity);
            }
        }
    }
    interface IPersonEvent : IApplicator<Person>
    {
    }

    record Person : Stream<Person, IPersonEvent>
    {
        public string? Name { get; set; }
        public string? FavoriteColor { get; set; }

        protected override Person Entity => this;

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

        public void Apply(Person entity)
        {
            entity.Name = Payload.Name;
            entity.FavoriteColor = Payload.FavoriteColor;
        }
    }

    record NameChangedEvent : Event<NameChange>, IPersonEvent
    {
        public NameChangedEvent(Guid Id, Guid StreamId, uint SequenceNumber, NameChange Payload)
            : base(Id, StreamId, SequenceNumber, Payload)
        {
        }

        public void Apply(Person entity)
        {
            entity.Name = Payload.Name;
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

        var tuples = SingleStream(streamId).OrderBy("sequence_number").Select("*").Get<application_event>();

        var person = new Person(streamId, tuples.Select(tup =>
        {
            if (tup.event_type == nameof(PersonInitializedEvent))
            {
                return (IPersonEvent)new PersonInitializedEvent(
                    Guid.Parse(tup.id), Guid.Parse(tup.stream_id), tup.sequence_number, JsonConvert.DeserializeObject<InitializePerson>(tup.payload) ?? throw new ApplicationException());
            }
            else if (tup.event_type == nameof(NameChangedEvent))
            {
                return (IPersonEvent)new NameChangedEvent(
                    Guid.Parse(tup.id), Guid.Parse(tup.stream_id), tup.sequence_number, JsonConvert.DeserializeObject<NameChange>(tup.payload) ?? throw new ApplicationException());
            }
            else
            {
                throw new ApplicationException("Invariant failed.");
            }
        }));
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
        _ = Events.Insert(new application_event
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