using CQRSlite.Commands;
using CQRSlite.Domain;
using CQRSlite.Events;
using CQRSlite.Messages;
using CQRSlite.Queries;
using CQRSlite.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using N3.CqrsEs.Händelser;
using N3.CqrsEs.LäsModell.Frågor;
using N3.CqrsEs.LäsModell.HändelseHantering;
using N3.CqrsEs.LäsModell.Infrastruktur;
using N3.CqrsEs.SkrivModell;
using N3.CqrsEs.SkrivModell.Hantering;
using N3.CqrsEs.SkrivModell.Kommando;
using N3.Modell;
using System.ComponentModel.Design;
using System.Reflection;

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
               //.UseServiceProviderFactory(new RouteRegistrarProviderFactory())
               .ConfigureServices((ctx, services) =>
               {
                   var router = new Router();
                   _ = services
                       .AddSingleton<Router>(router)
                       .AddSingleton<ICommandSender>(_ => router)
                       .AddSingleton<IEventPublisher>(_ => router)
                       .AddSingleton<IHandlerRegistrar>(_ => router)
                       .AddSingleton<IQueryProcessor>(_ => router)
                       .AddScoped<ISession, Session>()

                       .AddSingleton<IEventStore, InMemoryEventStore>()
                       .AddScoped<IRepository>(y => new Repository(y.GetRequiredService<IEventStore>()))

                       .AddScoped<InkassoÄrendeKommandoHanterare>()
                       .AddScoped<IInkassoÄrendeSession, TestInkassoÄrendeSession>()
                       .AddScoped(_ => new TestVyLagringDatabas())
                       .AddScoped<IVyLagring, TestVyLagring>()
                       .AddScoped<IÄrendeNummerUträknare, TestVyLagring>()
                       ;

                   _ = services.Scan(scan => scan
                        .FromAssemblies(typeof(InkassoÄrendeDetaljVy).GetTypeInfo().Assembly)
                            .AddClasses(classes => classes.Where(x =>
                            {
                                var allInterfaces = x.GetInterfaces();
                                return
                                    allInterfaces.Any(y => y.GetTypeInfo().IsGenericType && y.GetTypeInfo().GetGenericTypeDefinition() == typeof(IHandler<>)) ||
                                    allInterfaces.Any(y => y.GetTypeInfo().IsGenericType && y.GetTypeInfo().GetGenericTypeDefinition() == typeof(ICancellableHandler<>)) ||
                                    allInterfaces.Any(y => y.GetTypeInfo().IsGenericType && y.GetTypeInfo().GetGenericTypeDefinition() == typeof(IQueryHandler<,>)) ||
                                    allInterfaces.Any(y => y.GetTypeInfo().IsGenericType && y.GetTypeInfo().GetGenericTypeDefinition() == typeof(ICancellableQueryHandler<,>));
                            }))
                            .AsSelf()
                            .WithTransientLifetime()
                    );

                   _ = services.Scan(scan => scan
                        .FromAssemblies(typeof(SkapaInkassoÄrendeKommando).GetTypeInfo().Assembly)
                            .AddClasses(classes => classes.Where(x =>
                            {
                                var allInterfaces = x.GetInterfaces();
                                return
                                    allInterfaces.Any(y => y.GetTypeInfo().IsGenericType && y.GetTypeInfo().GetGenericTypeDefinition() == typeof(IHandler<>)) ||
                                    allInterfaces.Any(y => y.GetTypeInfo().IsGenericType && y.GetTypeInfo().GetGenericTypeDefinition() == typeof(ICancellableHandler<>)) ||
                                    allInterfaces.Any(y => y.GetTypeInfo().IsGenericType && y.GetTypeInfo().GetGenericTypeDefinition() == typeof(IQueryHandler<,>)) ||
                                    allInterfaces.Any(y => y.GetTypeInfo().IsGenericType && y.GetTypeInfo().GetGenericTypeDefinition() == typeof(ICancellableQueryHandler<,>));
                            }))
                            .AsSelf()
                            .WithTransientLifetime()
                    );
               })
               .Build();
            _scope = _host.Services.CreateAsyncScope();

            var registrar = new RouteRegistrar(_scope.ServiceProvider);
            registrar.RegisterInAssemblyOf(typeof(InkassoÄrendeKommandoHanterare));
        }

        T HämtaTjänst<T>()
            where T : notnull
        {
            return _scope.ServiceProvider.GetRequiredService<T>();
        }

        [Test]
        public async Task Test1()
        {
            var vyLagring = HämtaTjänst<IVyLagring>();
            var kommandoSkickare = HämtaTjänst<ICommandSender>();

            var kommando = new SkapaInkassoÄrendeKommando(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Array.Empty<UnikIdentifierare>(),
                Array.Empty<Faktura>());
            await kommandoSkickare.Send(kommando);

            var ärende = await vyLagring.HämtaSpecifiktÄrende(new HämtaSpecifiktInkassoÄrende(kommando.Identifierare));
            Assert.That(ärende, Is.Not.Null);
        }
    }
}