// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Akot.Database.Analyzer.Cli;
using Microsoft.Extensions.Configuration;

var host = Host.CreateDefaultBuilder()
    //.ConfigureAppConfiguration(app => { app.con})
    .ConfigureHostConfiguration(hostBuilder =>
    {
        hostBuilder.AddUserSecrets<Application>();
        hostBuilder.AddJsonFile("appsettings.json");
    })
    .ConfigureServices(services =>
    {
        services.AddTransient<TargetSqlConnectionStringResolver>();
        services.AddTransient<Storage>();
        services.AddTransient<Application>();
    })
    .Build();


var app = host.Services.GetRequiredService<Application>();
await app.RunAsync();
