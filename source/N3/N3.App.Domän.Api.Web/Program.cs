using Microsoft.AspNetCore.Http.Json;
using N3.Infrastruktur.Gemensam.Json;
using Newtonsoft.Json.Converters;
using N3.App.Domän.Api.Web.Controllers;
using N3.CqrsEs.Infrastruktur.Marten;
using System.Reflection;
using Marten.Services;

namespace N3.App.Domän.Api.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var svc = builder.Services;

            svc.AddBasicServices(builder.Configuration, builder.Environment);

            var app = builder.Build();

            // we use a path base here, because we expect to be reverse proxied via a path prefix
            _ = app.UsePathBase(new PathString("/api"));

            // Configure the HTTP request pipeline.
            ////if (app.Environment.IsDevelopment())
            ////{
            ////    _ = app.UseSwagger().UseSwaggerUI();
            ////}

            _ = app.UseHttpsRedirection();

            _ = app.UseAuthorization();

            _ = app.UseOpenApi().UseSwaggerUi3();

            _ = app.MapControllers();

            app.Run();
        }
    }
}
