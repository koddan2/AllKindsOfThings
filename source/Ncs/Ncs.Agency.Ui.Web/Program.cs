using EventStore.Client;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Ncs.EventSourcing;
using Ncs.Model.Configuration.Keycloak;
using SAK;
using SAK.Files;
using Serilog;
using System.Globalization;
using System.Reflection;

try
{
	Log.Logger = new LoggerConfiguration()
		.WriteTo.Console()
		.CreateLogger();

	var builder = WebApplication.CreateBuilder(args);

	_ = builder.Host.UseSerilog((ctx, lc) => lc
		.WriteTo.Console()
		.Enrich.FromLogContext()
		.ReadFrom.Configuration(ctx.Configuration));

	if (builder.Environment.IsDevelopment())
	{
		var thisAssemblyLocation = Assembly.GetExecutingAssembly().Location;
		var here = Path.GetDirectoryName(thisAssemblyLocation)!;
		var maybeRootDir = FileSystemWalker.FindFirstDirectoryContainingFileUpwards(here, x => x.Name == "Ncs.sln");
		if (maybeRootDir is string rootDir)
		{
			var commonIniFilePath = Path.Combine(rootDir, "Common", "appsettings.ini");
			_ = builder.Configuration.AddIniFile(commonIniFilePath);
		}
	}

	// Add services to the container.
	var services = builder.Services;

	_ = services.AddLogging(loggingBuilder =>
	{
		_ = loggingBuilder.AddSerilog(Log.Logger);
	});

	////var esdbConnString = builder.Configuration.GetValue<string>("EventStoreDB:ConnectionString");
	_ = services.AddNcsEventSourcingEventStoreDb(builder.Configuration.GetSection("EventStoreDB"));

	_ = services.AddLocalization(options => options.ResourcesPath = "Resources");
	var mvc = services.AddMvc()
	  .AddViewLocalization(LanguageViewLocationExpanderFormat.SubFolder)
	  .AddDataAnnotationsLocalization();
	////var mvc = services.AddRazorPages();
	if (builder.Environment.IsDevelopment())
	{
		_ = mvc.AddRazorRuntimeCompilation();
	}
	_ = services
		.AddAuthentication(options =>
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

			//var a = options.ClaimActions;

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
	var supportedCultures = new List<CultureInfo>
	{
	   new CultureInfo("sv-SE"),
	   new CultureInfo("en-US"),
	};

	_ = app.UseRequestLocalization(new RequestLocalizationOptions
	{
		DefaultRequestCulture = new RequestCulture("sv-SE"),
		SupportedCultures = supportedCultures,
		SupportedUICultures = supportedCultures,
	});

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

	//_ = app.MapRazorPages();
	_ = app.MapDefaultControllerRoute();
	_ = app.MapControllers();

	app.Run();
}
catch (Exception e)
{
	Log.Error(e, "Fatal");
	throw;
}
finally
{
	Log.CloseAndFlush();
}
