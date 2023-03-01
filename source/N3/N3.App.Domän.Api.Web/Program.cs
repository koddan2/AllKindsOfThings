using Microsoft.AspNetCore.Http.Json;
using Microsoft.OpenApi.Models;
using N3.App.Domän.Api.Web.Vanligt;
using N3.Infrastruktur.Gemensam.Json;
using N3.Låtsas;
using System.ComponentModel;

namespace N3.App.Domän.Api.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            _ = builder.Services.AddControllers()
            ////.AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new UnikIdentifierareJsonConverter()))
            ;

            _ = builder.Services.Configure<JsonOptions>(options =>
            {
                ////options.SerializerOptions.Encoder = null;
                options.SerializerOptions.Converters.Add(new UnikIdentifierareJsonConverter());
                options.SerializerOptions.AddDateOnlyConverters();
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            _ = builder.Services.AddEndpointsApiExplorer();
            _ = builder.Services.AddSwaggerGen(options =>
            {
                options.MapType<UnikIdentifierare>(() => new OpenApiSchema { Type = "string", });
                options.MapType<DateOnly>(() => new OpenApiSchema { Type = "string", });
            });

            var svc = builder.Services;
            _ = svc.InstalleraLåtsasTjänster(
                builder.Configuration.GetRequiredSection("Låtsas"),
                builder.Environment
            );

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                _ = app.UseSwagger().UseSwaggerUI();
            }

            _ = app.UseHttpsRedirection();

            _ = app.UseAuthorization();

            _ = app.MapControllers();

            app.Run();
        }
    }
}
