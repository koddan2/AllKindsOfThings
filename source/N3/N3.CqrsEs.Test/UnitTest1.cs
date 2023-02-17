using Cqrs.Commands;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using N3.CqrsEs.LäsModell.Frågor;
using N3.CqrsEs.LäsModell.Infrastruktur;
using N3.CqrsEs.SkrivModell;
using N3.CqrsEs.SkrivModell.Hantering;
using N3.CqrsEs.SkrivModell.Kommando;
using N3.CqrsEs.Test.TestTjänster;
using N3.Modell;
using System.Runtime.CompilerServices;
using N3.CqrsEs.LäsModell.DataÖverföring;
using Cqrs.Domain;
using Cqrs.Domain.Factories;
using Cqrs.Configuration;
using Chinchilla.Logging;
using Cqrs.Events;
using Chinchilla.StateManagement;
using Chinchilla.StateManagement.Threaded;
using Chinchilla.Logging.Configuration;

namespace N3.CqrsEs.Test
{
    public class TestDependencyResolver : DependencyResolver
    {
        private readonly IServiceProvider _provider;

        public TestDependencyResolver(IServiceProvider provider)
        {
            _provider = provider;
        }

        public override T Resolve<T>()
        {
            return _provider.GetRequiredService<T>();
        }

        public override object Resolve(Type type)
        {
            return _provider.GetRequiredService(type);
        }
    }
    [TestFixture]
    public class ÄrendeAggregatsTester
    {
        private IHost _host;
        private IServiceScope _scope;

        [SetUp]
        public void Setup()
        {
            _host = new HostBuilder()
               .ConfigureServices((ctx, services) =>
               {
                   _ = services

                       .AddScoped<ILogger, ConsoleLogger>()
                       .AddScoped<ILoggerSettings>((_)=>new LoggerSettings())
                       .AddScoped<ICorrelationIdHelper, CorrelationIdHelper>()
                       .AddScoped<IContextItemCollectionFactory, ContextItemCollectionFactory>()
                       .AddScoped<IConfigurationManager, ConfigurationManager>()

                       .AddScoped<IDependencyResolver, TestDependencyResolver>()
                       .AddScoped<IAggregateFactory, AggregateFactory>()
                       .AddScoped<IAggregateRepository<string>, AggregateRepository<string>>()
                       .AddScoped<IUnitOfWork<string>, UnitOfWork<string>>()

                       .AddScoped<IEventStore<string>, InMemoryEventStore>()
                       .AddScoped<IEventPublisher<string>, InMemoryEventStore>()


                       .AddScoped<TestVyLagringDatabas>()

                       .AddScoped<IVyLagring, TestVyLagring>()
                       .AddScoped<IÄrendeNummerUträknare, TestVyLagring>()

                       .AddScoped<IFrågeHanterare<HämtaSpecifiktInkassoÄrende, InkassoÄrendeFullVyModell>, TestFrågeHanterare>()
                       .AddScoped<IInkassoÄrendeKommandoHanterare, InkassoÄrendeKommandoHanterare>()
                       ;
               })
               .Build();

            _scope = _host.Services.CreateAsyncScope();
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        public async Task SkapaÄrende1(int _)
        {
            var kommandoSkickare = _scope.Plocka<IInkassoÄrendeKommandoHanterare>();
            var frågeHanterare = _scope.Plocka<IFrågeHanterare<HämtaSpecifiktInkassoÄrende, InkassoÄrendeFullVyModell>>();

            var faktura = new Faktura(
                FakturaNummer: "4155",
                UrsprungligBetalReferens: "9842395923",
                FakturaDatum: DateTime.Now.AddDays(-100).ToDateOnly(),
                FörfalloDatum: DateTime.Now.AddDays(-70).ToDateOnly(),
                UrsprungligtKapitalBelopp: new SvenskaKronor(8582),
                KvarvarandeKapitalBelopp: new SvenskaKronor(8004),
                RänteSats: new Procent(8),
                RänteUträkningsSätt: RänteUträkningsSätt.DagligUträkningPåÅrsbasis,
                RänteSatsTyp: RänteSatsTyp.ÖverGällandeReferensRänta,
                Kostnader: new[]
                {
                    SkuldElement.PåminnelseKostnad,
                },
                RänteStoppsDatum: null);

            var kommando = new SkapaInkassoÄrendeKommando(
                Guid.NewGuid(),
                Guid.NewGuid(),
                new[] { (UnikIdentifierare)Guid.NewGuid() },
                new[] { faktura });
            kommandoSkickare.Handle(kommando);

            var ärende = await frågeHanterare.Hantera(new HämtaSpecifiktInkassoÄrende(kommando.Identifierare));
            Assert.That(ärende, Is.Not.Null);
            Assert.That(ärende.Fakturor, Has.Length.EqualTo(1));
            Assert.That(ärende.Fakturor[0].FakturaNummer, Is.EqualTo(faktura.FakturaNummer));
        }

        [Test]
        public async Task SkapaÄrendeDubblettHindras1()
        {
            await ValueTask.CompletedTask;
            ////var kommandoSkickare = _scope.Plocka<ICommandSender>();
            ////var frågeHanterare = _scope.Plocka<IQueryProcessor>();

            ////var kommando = new SkapaInkassoÄrendeKommando(
            ////    Guid.NewGuid(),
            ////    Guid.NewGuid(),
            ////    Array.Empty<UnikIdentifierare>(),
            ////    Array.Empty<Faktura>());
            ////await kommandoSkickare.Send(kommando);
            ////var ex = Assert.ThrowsAsync<AggregatExisterarRedanException>(async () =>
            ////{
            ////    await kommandoSkickare.Send(kommando);
            ////});

            ////Assert.That((UnikIdentifierare)ex.Id, Is.EqualTo(kommando.Identifierare));
        }

        [Test]
        public async Task SkapaÄrendeFlera1()
        {
            await ValueTask.CompletedTask;
            ////var kommandoSkickare = _scope.Plocka<ICommandSender>();
            ////var frågeHanterare = _scope.Plocka<IQueryProcessor>();

            ////var räknare = 0;
            ////foreach (var item in Enumerable.Range(1, 10))
            ////{
            ////    var kommando = new SkapaInkassoÄrendeKommando(
            ////        Guid.NewGuid(),
            ////        Guid.NewGuid(),
            ////        Array.Empty<UnikIdentifierare>(),
            ////        Array.Empty<Faktura>());
            ////    await kommandoSkickare.Send(kommando);
            ////    räknare++;
            ////    var ex = Assert.ThrowsAsync<AggregatExisterarRedanException>(async () =>
            ////    {
            ////        await kommandoSkickare.Send(kommando);
            ////    });

            ////    Assert.That((UnikIdentifierare)ex.Id, Is.EqualTo(kommando.Identifierare));
            ////    var ärende = await frågeHanterare.Query(new HämtaSpecifiktInkassoÄrende(kommando.Identifierare));
            ////}

            ////var db = _scope.ServiceProvider.GetRequiredService<TestVyLagringDatabas>();
            ////Assert.That(db.Ärenden, Has.Count.EqualTo(räknare));
        }
    }
}