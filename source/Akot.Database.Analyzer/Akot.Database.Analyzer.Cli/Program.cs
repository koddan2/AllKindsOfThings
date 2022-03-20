// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.DependencyInjection;
using Akot.Database.Analyzer.Cli;
using Microsoft.Extensions.Configuration;

var host = Host.CreateDefaultBuilder()
    //.ConfigureAppConfiguration(app => { app.con})
    .ConfigureHostConfiguration(hostBuilder => hostBuilder.AddUserSecrets<App>())
    .ConfigureServices(services =>
    {
        services.AddTransient<ConnString>();
        services.AddTransient<App>();
    })
    .Build();


var app = host.Services.GetRequiredService<App>();
await app.RunAsync();
