using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using N3.Modell;
using N3.CqrsEs.Ramverk;
using N3.CqrsEs.LäsModell;
using N3.CqrsEs.LäsModell.Frågor;
using N3.CqrsEs.LäsModell.DataÖverföring;
using N3.CqrsEs.SkrivModell;
using N3.CqrsEs.SkrivModell.Kommando;
using N3.CqrsEs.Test.TestTjänster;

namespace N3.CqrsEs.Test
{
    [TestFixture]
    public class ÄrendeAggregatsTester
    {
        private IHost _host;
        private IServiceScope _scope;

        [SetUp]
        public void Setup()
        {
            _host = new HostBuilder()
                .ConfigureServices(
                    (ctx, services) =>
                    {
                        _ = services
                            .LäggTillLäsModell(ctx.Configuration)
                            .LäggTillSkrivModell(ctx.Configuration)
                            .RegistreraTestTjänster()
                            .AddScoped<
                                IFrågeHanterare<
                                    HämtaSpecifiktInkassoÄrende,
                                    InkassoÄrendeFullVyModell
                                >,
                                TestFrågeHanterare
                            >();
                    }
                )
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
            var kommandoSkickare = _scope.Plocka<IKommandoHanterare<SkapaInkassoÄrendeKommando>>();
            var frågeHanterare = _scope.Plocka<
                IFrågeHanterare<HämtaSpecifiktInkassoÄrende, InkassoÄrendeFullVyModell>
            >();

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
                Kostnader: new[] { SkuldElement.PåminnelseKostnad, },
                RänteStoppsDatum: null
            );

            var kommando = new SkapaInkassoÄrendeKommando(
                Guid.NewGuid(),
                Guid.NewGuid(),
                new[] { (UnikIdentifierare)Guid.NewGuid() },
                new[] { faktura }
            );
            await kommandoSkickare.Hantera(kommando);
            var fråga = new HämtaSpecifiktInkassoÄrende(
                kommando.AggregatIdentifierare,
                UnikIdentifierare.Skapa()
            );
            var ärende = await frågeHanterare.Hantera(fråga);
            Assert.That(ärende, Is.Not.Null);
            Assert.That(ärende.Fakturor, Has.Length.EqualTo(1));
            Assert.That(ärende.Fakturor[0].FakturaNummer, Is.EqualTo(faktura.FakturaNummer));
        }

        [Test]
        public async Task SkapaÄrendeDubblettHindras1()
        {
            ////await ValueTask.CompletedTask;
            var kommandoSkickare = _scope.Plocka<IKommandoHanterare<SkapaInkassoÄrendeKommando>>();
            var frågeHanterare = _scope.Plocka<
                IFrågeHanterare<HämtaSpecifiktInkassoÄrende, InkassoÄrendeFullVyModell>
            >();

            var kommando = new SkapaInkassoÄrendeKommando(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Array.Empty<UnikIdentifierare>(),
                Array.Empty<Faktura>()
            );
            await kommandoSkickare.Hantera(kommando);
            var ex = Assert.ThrowsAsync<AggregatExisterarRedanException>(async () =>
            {
                await kommandoSkickare.Hantera(kommando);
            });

            Assert.That(ex.AggregatIdentifierare, Is.EqualTo(kommando.AggregatIdentifierare));
        }

        [Test]
        public async Task SkapaÄrendeFlera1()
        {
            ////await ValueTask.CompletedTask;
            var kommandoSkickareSkapa = _scope.Plocka<
                IKommandoHanterare<SkapaInkassoÄrendeKommando>
            >();
            var kommandoSkickareTilldelaÄrendeNummer = _scope.Plocka<
                IKommandoHanterare<TilldelaÄrendeNummerTillInkassoÄrendeKommando>
            >();
            var frågeHanterare = _scope.Plocka<
                IFrågeHanterare<HämtaSpecifiktInkassoÄrende, InkassoÄrendeFullVyModell>
            >();

            var räknare = 0;
            foreach (var item in Enumerable.Range(1, 10))
            {
                UnikIdentifierare aggId;
                {
                    var kommando = new SkapaInkassoÄrendeKommando(
                        Guid.NewGuid(),
                        Guid.NewGuid(),
                        Array.Empty<UnikIdentifierare>(),
                        Array.Empty<Faktura>()
                    );
                    await kommandoSkickareSkapa.Hantera(kommando);
                    aggId = kommando.AggregatIdentifierare;
                }
                räknare++;
                {
                    var kommando = new TilldelaÄrendeNummerTillInkassoÄrendeKommando(
                        aggId,
                        räknare
                    );
                    await kommandoSkickareTilldelaÄrendeNummer.Hantera(kommando);
                }

                var fråga = new HämtaSpecifiktInkassoÄrende(aggId, UnikIdentifierare.Skapa());
                var ärende = await frågeHanterare.Hantera(fråga);
                Assert.That(ärende.ÄrendeNummer, Is.EqualTo(räknare));
            }

            var db = _scope.ServiceProvider.GetRequiredService<TestVyLagringDatabas>();
            Assert.That(db.Ärenden, Has.Count.EqualTo(räknare));
        }
    }
}
