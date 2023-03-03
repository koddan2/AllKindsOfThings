using Microsoft.AspNetCore.Http.Json;
using N3.Infrastruktur.Gemensam.Json;
using Newtonsoft.Json.Converters;
using N3.App.Domän.Api.Web.Controllers;
using N3.CqrsEs.Infrastruktur.Marten;

namespace N3.App.Domän.Api.Web
{
    public static class Topics
    {
        public const string ImporteraÄrende = "importera-ärende";
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var svc = builder.Services;

            _ = svc.AddControllers()
            ////.AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new UnikIdentifierareJsonConverter()))
            ;

            _ = svc.Configure<JsonOptions>(options =>
            {
                ////options.SerializerOptions.Encoder = null;
                options.SerializerOptions.Converters.Add(new UnikIdentifierareJsonConverter());
                options.SerializerOptions.AddDateOnlyConverters();
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            _ = svc.AddEndpointsApiExplorer();
            ////_ = builder.Services.AddSwaggerGen(options =>
            ////{
            ////    options.MapType<UnikIdentifierare>(() => new OpenApiSchema { Type = "string", });
            ////    options.MapType<DateOnly>(() => new OpenApiSchema { Type = "string", });

            ////    options.SwaggerDoc(
            ////        "v1",
            ////        new OpenApiInfo
            ////        {
            ////            Description = "Version 1",
            ////            Title = "Version 1",
            ////            Version = "v1",
            ////        }
            ////    );
            ////});

            ////_ = svc.InstalleraLåtsasTjänster(
            ////    builder.Configuration.GetRequiredSection("Låtsas"),
            ////    builder.Environment
            ////);

            _ = svc.LäggTillCqrsEsInfrastrukturMarten(builder.Configuration, builder.Environment);

            _ = svc.AddMemoryCache(opts =>
            {
                ////opts.
            });

            _ = svc.AddSwaggerDocument(cfg =>
            {
                cfg.ApiGroupNames = new[] { "v1" };
                cfg.SerializerSettings = new Newtonsoft.Json.JsonSerializerSettings()
                {
                    Converters = { new StringEnumConverter(), }
                };
            });

            ////svc.AddHostedService<BusAccumulator>();

            var app = builder.Build();

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
