using Bogus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using N3.CqrsEs.Gemensam.Händelser;
using N3.CqrsEs.Infrastruktur.Marten;
using N3.CqrsEs.Ramverk;
using N3.CqrsEs.SkrivModell.Domän;
using N3.Modell;

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
        public async Task Test1()
        {
            var faker = new Faker();
            var hk = _scope.ServiceProvider.GetRequiredService<IHändelseKassa>();
            var aggRepo = _scope.ServiceProvider.GetRequiredService<AggregateRepository>();
            var agg = new InkassoÄrende(UnikIdentifierare.Skapa());

            var klientref = UnikIdentifierare.Skapa();
            var glref = new UnikIdentifierare[] { UnikIdentifierare.Skapa() };
            var fakturor = new Faktura[] { Generatorer.BasGeneratorer.TestFakturor.Generate() };
            agg.SkapaÄrende(klientref, glref, fakturor);
            await aggRepo.StoreAsync(agg);
            ////await hk.Registrera(agg.TillStrömIdentifierare(), event1, HändelseModus.SkapaNy);

            var agg2 = await aggRepo.LoadAsync<InkassoÄrende>(agg.Id);
            var ärendeNummer = faker.Random.Int(100, 99999);
            agg2.TilldelaÄrendeNummer(ärendeNummer);
            await aggRepo.StoreAsync(agg2);
            ////await hk.Registrera(agg.TillStrömIdentifierare(), @event, HändelseModus.SkapaNy);
            ////var event2 = new InkassoÄrendeBlevTilldelatÄrendeNummer(agg.Identifierare, 101);
            ////await hk.Registrera(agg.TillStrömIdentifierare(), event2, HändelseModus.LäggTill);
        }
    }
}
