using Yarp.ReverseProxy.Transforms;

namespace N3.App.Ombud.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            _ = builder.Services.AddControllersWithViews();

            var svc = builder.Services;
            _ = svc.AddReverseProxy()
                .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
                .AddTransforms(ctx =>
                {
                    //_ = ctx.AddPathRemovePrefix(new PathString("/api"));
                    _ = ctx.AddRequestTransform(async (req) =>
                    {
                        await ValueTask.CompletedTask;
                        req.Path = new PathString(req.Path.Value!.Replace("/api", ""));
                    });
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                _ = app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                _ = app.UseHsts();
            }

            _ = app.UseHttpsRedirection();
            _ = app.UseStaticFiles();

            _ = app.UseRouting();

            _ = app.UseAuthorization();

            _ = app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );

            _ = app.UseEndpoints(endpoints =>
            {
                endpoints.MapReverseProxy();
            });

            app.Run();
        }
    }
}
