using CQRSlite.Commands;
using CQRSlite.Queries;
using CQRSlite.Routing;
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
    public sealed class TestAssemblyMarker { }
    public class Tests
    {
        private IHost _host;
        private IServiceScope _scope;

        [SetUp]
        public void Setup()
        {
            _host = new HostBuilder()
               .UseServiceProviderFactory(new RouteRegistrarProviderFactory())
               .RegisterCqrsServices(
                    typeof(TestAssemblyMarker),
                    typeof(CqrsEsAssemblyMarker))
               .ConfigureServices((ctx, services) =>
               {
                   _ = services
                       .AddScoped<IInkassoÄrendeSession, TestInkassoÄrendeSession>()
                       .AddScoped(_ => new TestVyLagringDatabas())
                       .AddScoped<IVyLagring, TestVyLagring>()
                       .AddScoped<IÄrendeNummerUträknare, TestVyLagring>()
                       ;
               })
               .Build();

            _scope = _host.Services.CreateAsyncScope();
        }

        [Test]
        public async Task SkapaÄrende1()
        {
            var kommandoSkickare = _scope.Plocka<ICommandSender>();
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
                new[] {faktura} );
            await kommandoSkickare.Send(kommando);

            var ärende = await frågeHanterare.Query(new HämtaSpecifiktInkassoÄrende(kommando.Identifierare));
            Assert.That(ärende, Is.Not.Null);
            Assert.That(ärende.Fakturor, Has.Length.EqualTo(1));
            Assert.That(ärende.Fakturor[0].FakturaNummer, Is.EqualTo(faktura.FakturaNummer));
        }

        [Test]
        public async Task Test2()
        {
            var kommandoSkickare = _scope.Plocka<ICommandSender>();
            var frågeHanterare = _scope.Plocka<IQueryProcessor>();

            var kommando = new SkapaInkassoÄrendeKommando(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Array.Empty<UnikIdentifierare>(),
                Array.Empty<Faktura>());
            await kommandoSkickare.Send(kommando);

            var ärende = frågeHanterare.Query(new HämtaSpecifiktInkassoÄrende(kommando.Identifierare));
            Assert.That(ärende, Is.Not.Null);
        }
    }
}