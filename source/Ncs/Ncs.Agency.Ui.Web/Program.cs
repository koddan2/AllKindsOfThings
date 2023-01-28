
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Ncs.Model.Configuration;
using SAK;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
_ = services.AddRazorPages();
_ = services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        ////options.DefaultSignInScheme = OpenIdConnectDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddJwtBearer(options =>
    {
        options.IncludeErrorDetails = true;
    })
    .AddOpenIdConnect(options =>
    {
        options.AuthenticationMethod = OpenIdConnectRedirectBehavior.FormPost;
        options.ResponseType = "code";
        var sect = builder.Configuration.GetSection("Keycloak");
        var c = sect
            .Get<KeycloakConfiguration>().OrFail();
        options.ClientId = c.Resource;
        options.ClientSecret = c.Credentials.OrFail().Secret;
        var authority = $"{c.Auth_Server_Url}realms/{c.Realm}";
        options.Authority = new Uri(authority).ToString();
        options.GetClaimsFromUserInfoEndpoint = true;
        if (builder.Environment.IsDevelopment())
        {
            options.RequireHttpsMetadata = true;
            HttpClientHandler handler = new()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            options.BackchannelHttpHandler = handler;
        }
        else
        {
            options.RequireHttpsMetadata = true;
        }
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    _ = app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    _ = app.UseHsts();
}

_ = app.UseHttpsRedirection();
_ = app.UseStaticFiles();

_ = app.UseRouting();

_ = app.UseAuthorization();

_ = app.MapRazorPages();

try
{

    app.Run();
}
catch (Exception e)
{
    var exn = e;
    throw;
}
