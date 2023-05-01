namespace LeanMachineT1
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            //app.UseAuthorization();

            //app.MapGet("/", async (context) =>
            //{
            //    context.Request.Query.TryGetValue("message", out var message);
            //    context.Response.StatusCode = 200;
            //    await context.Response.WriteAsync($"<h1>{message}</h1>");
            //    await context.Response.CompleteAsync();
            //});

            app.MapRazorPages();

            app.Run();
        }
    }
}