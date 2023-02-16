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

namespace N3.CqrsEs.Test
{
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
                       .AddScoped<TestVyLagringDatabas>()
                       .AddScoped<IVyLagring, TestVyLagring>()
                       .AddScoped<IÄrendeNummerUträknare, TestVyLagring>()
                       ;
               })
               .Build();

            _scope = _host.Services.CreateAsyncScope();
        }
        void UnhandledException(Exception exception)
        {
            _ = exception.Demystify();
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
            var kommandoSkickare = _scope.Plocka<ICommandPublisher<string>>();
            var frågeHanterare = _scope.Plocka<IQueryProcessor>();

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
            kommandoSkickare.Publish(kommando);

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