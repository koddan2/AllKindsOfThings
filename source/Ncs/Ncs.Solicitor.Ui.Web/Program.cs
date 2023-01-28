using Microsoft.EntityFrameworkCore;
//using Ncs.Solicitor.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
_ = services.AddRazorPages();
////_ = services.AddDbContext<NcsSolicitorDataContext>(options =>
////{
////    _ = options.UseNpgsql("Host=localhost;Username=ncsusr;Password=abc123;Database=postgres;");
////});

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

app.Run();
