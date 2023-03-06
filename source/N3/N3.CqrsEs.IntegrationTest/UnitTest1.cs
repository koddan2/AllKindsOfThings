using Bogus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using N3.CqrsEs.Infrastruktur.Marten;
using N3.CqrsEs.Ramverk;
using N3.CqrsEs.Ramverk.Jobs;
using N3.CqrsEs.SkrivModell.Domän;
using N3.Modell;

[assembly: LevelOfParallelism(4)]

namespace N3.CqrsEs.IntegrationTest
{
    public class Tests
    {
        static Tests()
        {
            Faker.DefaultStrictMode = true;
        }

        private IHost _host;
        private IServiceScope _scope;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _host = new HostBuilder()
                .ConfigureAppConfiguration(
                    (ctx, builder) =>
                    {
                        _ = builder.AddIniFile("appsettings.ini", false);
                    }
                )
                .ConfigureServices(
                    (ctx, services) =>
                    {
                        _ = services.LäggTillCqrsEsInfrastrukturMarten(
                            ctx.Configuration,
                            ctx.HostingEnvironment
                        );
                    }
                )
                .Build();
        }

        [SetUp]
        public void Setup()
        {
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
        [TestCase(8)]
        [TestCase(9)]
        [TestCase(10)]
        [TestCase(11)]
        [TestCase(12)]
        public async Task Test1(object _)
        {
            var faker = new Faker();
            var hk = _scope.ServiceProvider.GetRequiredService<IHändelseKassa>();
            var aggRepo = _scope.ServiceProvider.GetRequiredService<AggregateRepository>();
            var agg = new InkassoÄrende(UnikIdentifierare.Skapa());

            string klientref = UnikIdentifierare.Skapa();
            var glref = new string[] { (string)UnikIdentifierare.Skapa() };
            var fakturor = new Faktura[] { Generatorer.BasGeneratorer.TestFakturor.Generate() };
            agg.SkapaÄrende(klientref, glref, fakturor);
            await aggRepo.StoreAsync(agg);

            var agg2 = await aggRepo.LoadAsync<InkassoÄrende>(agg.Id);
            var ärendeNummer = faker.Random.Int(100, 99999);
            agg2.TilldelaÄrendeNummer(ärendeNummer);
            await aggRepo.StoreAsync(agg2);
            Assert.Multiple(() =>
            {
                Assert.That(agg2.Id, Is.EqualTo(agg.Id));
                Assert.That(agg2.ÄrendeNummer, Is.EqualTo(ärendeNummer));
            });
        }

        public class TestJobbTyp : AbstractJob
        {
            public int Nummer { get; set; }
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        [TestCase(8)]
        [TestCase(9)]
        [TestCase(10)]
        [TestCase(11)]
        [TestCase(12)]
        public async Task TestBuss1(object _)
        {
            var buss = _scope.ServiceProvider.GetRequiredService<IJobQueue>();
            var sak = new TestJobbTyp { Id = UnikIdentifierare.Skapa(), Nummer = 8 };
            await buss.Queue(sak);
            var status = await buss.GetStatus<TestJobbTyp>(sak.Id);
            Assert.That(status, Is.Not.Null);
            var reservation = await buss.Reserve<TestJobbTyp>(sak.Id);
            Assert.That(reservation, Is.Not.Null);
            await buss.Dequeue<TestJobbTyp>(sak.Id, reservation);
        }
    }
}
