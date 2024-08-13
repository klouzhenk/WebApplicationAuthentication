using AuthenticationUI.Components;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using WebApplicationShared.Helpers;
using WebApplicationShared;

using API.Services.Implementation.HttpClients;
using API.Services.Interfaces.HttpClients;
using API.Services.Implementation.DataServices;
using API.Services.Interfaces.DataServices;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog(); // Add this line

// Add services to the container.
builder.Services.AddLocalization();
builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers().AddMvcOptions(x => x.CacheProfiles.Clear());

builder.Services.AddHttpClient<IUserAPIClient, UserAPIClient>(configureClient =>
{
    configureClient.BaseAddress = new Uri("https://localhost:7267");
});
builder.Services.AddTransient<IUserDataService, UserDataService>();

builder.Services.AddHttpClient<IWeatherForecastAPIClient, WeatherForecastAPIClient>(configureClient =>
{
    configureClient.BaseAddress = new Uri("https://localhost:7267");
});
builder.Services.AddTransient<IWeatherForecastDataService, WeatherForecastDataService>();

builder.Services.AddAuthenticationCore();
builder.Services.AddAuthorizationCore();

// Register CustomAuthStateProvider from the WebApplicationShared RCL
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

//builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddSingleton<JwtSecurityTokenHandler>();
//builder.Services.AddScoped<DeleteAccountService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => builder.Configuration.Bind("JwtSettings", options));

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

// Configure HttpClient
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7267")
});

var app = builder.Build();

string[] supportedCultures = { "en-US", "uk-UA" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);
app.UseRequestLocalization(localizationOptions);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(WebApplicationShared._Imports).Assembly);
//app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();