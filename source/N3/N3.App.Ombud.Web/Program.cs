using N3.App.Domän.Api.Web;
using N3.App.Domän.Api.Web.Controllers;
using Rebus.Config;
using Rebus.Routing.TypeBased;

namespace N3.App.Ombud.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var svc = builder.Services;
            _ = svc.AddRebus(
                configure =>
                    configure
                        .Transport(
                            t =>
                                t.UsePostgreSql(
                                    builder.Configuration.GetConnectionString("Rebus"),
                                    "rebus_transport",
                                    "inpq"
                                )
                        )
                        .Routing(
                            r =>
                                r.TypeBased()
                                    .Map<ImporteraInkassoÄrendeModell>(Topics.ImporteraÄrende)
                        )
                        .Subscriptions(s =>
                        {
                            s.StoreInPostgres(
                                builder.Configuration.GetConnectionString("Rebus"),
                                "rebus_subscriptions",
                                isCentralized: true
                            );
                        })
                        , onCreated: async bus =>
                        {
                            await bus.Subscribe<ImporteraInkassoÄrendeModell>();
                        }
            )
                .AddRebusHandler<ImporteraInkassoÄrendeHanterare>()
                .AddRebusHandler<ImporteraInkassoÄrendeHanterare2>()
                ;

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );

            app.Run();
        }
    }
}
