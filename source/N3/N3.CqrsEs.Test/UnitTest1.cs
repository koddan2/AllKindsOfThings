using CQRSlite.Commands;
using CQRSlite.Domain;
using CQRSlite.Events;
using CQRSlite.Queries;
using CQRSlite.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using N3.CqrsEs.L�sModell.Data�verf�ring;
using N3.CqrsEs.L�sModell.Fr�gor;
using N3.CqrsEs.L�sModell.Infrastruktur;
using N3.CqrsEs.SkrivModell;
using N3.CqrsEs.SkrivModell.Hantering;
using N3.Modell;

namespace N3.CqrsEs.Test
{
    public class Tests
    {
        private IHost _host;
        private AsyncServiceScope _scope;

        [SetUp]
        public void Setup()
        {
            _host = new HostBuilder()
               .ConfigureServices((ctx, services) =>
               {
                   _ = services
                       .AddScoped<ICommandSender, Router>()
                       .AddScoped<IEventPublisher, Router>()
                       .AddScoped<IQueryProcessor, Router>()
                       .AddScoped<IHandlerRegistrar, Router>()
                       .AddScoped<Inkasso�rendeKommandoHanterare>()
                       .AddScoped<IEventStore, TestEventStore>()
                       .AddScoped<ISession, Session>()
                       .AddScoped<IRepository, Repository>()
                       .AddScoped<IInkasso�rendeSession, TestInkasso�rendeSession>()
                       .AddScoped<IVyLagring, TestVyLagring>()
                       .AddScoped<I�rendeNummerUtr�knare, TestVyLagring>()
                       ;
               })
               .Build();
            _scope = _host.Services.CreateAsyncScope();
        }

        T H�mtaTj�nst<T>()
            where T : notnull
        {
            return _scope.ServiceProvider.GetRequiredService<T>();
        }

        [Test]
        public async Task Test1()
        {
            var kommandoHanterare = H�mtaTj�nst<Inkasso�rendeKommandoHanterare>();
            var vyLagring = _scope.ServiceProvider.GetRequiredService<IVyLagring>();

            var kommando = new SkrivModell.Kommando.SkapaInkasso�rendeKommando(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Array.Empty<UnikIdentifierare>(),
                Array.Empty<Faktura>());
            await kommandoHanterare.Handle(kommando);

            var �rende = await vyLagring.H�mtaSpecifikt�rende(new H�mtaSpecifiktInkasso�rende(kommando.Identifierare));
            Assert.That(�rende, Is.Not.Null);
        }
    }

    internal class TestEventStore : IEventStore
    {
        private readonly Dictionary<Guid, List<IEvent>> _lagring = new();
        public async Task<IEnumerable<IEvent>> Get(Guid aggregateId, int fromVersion, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            if (!_lagring.ContainsKey(aggregateId))
            {
                _lagring[aggregateId] = new List<IEvent>();
            }
            return _lagring[aggregateId].Skip(fromVersion);
        }

        public async Task Save(IEnumerable<IEvent> events, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            foreach (var item in events)
            {
                if (!_lagring.ContainsKey(item.Id))
                {
                    _lagring[item.Id] = new List<IEvent>();
                }
                _lagring[item.Id].Add(item);
            }
        }
    }

    internal class TestInkasso�rendeSession : IInkasso�rendeSession
    {
        private readonly ISession _session;

        public TestInkasso�rendeSession(ISession session)
        {
            _session = session;
        }

        public async Task Add<T>(T aggregate, CancellationToken cancellationToken = default) where T : AggregateRoot
        {
            await _session.Add(aggregate, cancellationToken);
        }

        public async Task Commit(CancellationToken cancellationToken = default)
        {
            await _session.Commit(cancellationToken);
        }

        public async Task<T> Get<T>(Guid id, int? expectedVersion = null, CancellationToken cancellationToken = default) where T : AggregateRoot
        {
            return await _session.Get<T>(id, expectedVersion, cancellationToken);
        }
    }

    internal class TestVyLagring : IVyLagring, I�rendeNummerUtr�knare
    {
        private readonly Dictionary<string, Inkasso�rendeFullVyModell> _fullVyLaging = new();

        public async Task<Inkasso�rendeFullVyModell> H�mtaSpecifikt�rende(H�mtaSpecifiktInkasso�rende parametrar, CancellationToken token = default)
        {
            await Task.CompletedTask;
            return _fullVyLaging[parametrar.�rendeIdentifierare];
        }

        public async Task L�ggTill�rende(Inkasso�rendeFullVyModell inkasso�rendeFullVyModell, CancellationToken token = default)
        {
            await Task.CompletedTask;
            _fullVyLaging.Add(inkasso�rendeFullVyModell.�rendeIdentifierare, inkasso�rendeFullVyModell);
        }

        public async Task<long> TaFramN�staLediga�rendeNummer()
        {
            await Task.CompletedTask;
            return _fullVyLaging.Values.Count + 1;
        }
    }
}