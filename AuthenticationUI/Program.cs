using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;
using AuthenticationUI.Components;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using API.Services.Interfaces.HttpClients;
using API.Services.Implementation.HttpClients;
using API.Services.Interfaces.DataServices;
using API.Services.Implementation.DataServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLocalization();
builder.Services.AddControllers();

// Додаємо послуги до контейнера
builder.Services.AddRazorPages();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

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

builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddSingleton<JwtSecurityTokenHandler>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => builder.Configuration.Bind("JwtSettings", options));

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

// Налаштування HttpClient
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7267")
});

var app = builder.Build();



string[] supportedCultures = ["en-US", "uk-UA"];
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);
app.UseRequestLocalization(localizationOptions);



// Налаштування конвеєра обробки запитів
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
    .AddInteractiveServerRenderMode();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();



static void AddLocalization(WebApplicationBuilder builder)
{
    builder.Services.AddLocalization(options => options.ResourcesPath = "Locales");

    var supportedCultures = new List<CultureInfo>();
    var cultures = builder.Configuration.GetSection("Cultures").GetChildren().ToDictionary(x => x.Key, x => x.Value);
    foreach (var culture in cultures)
        supportedCultures.Add(new CultureInfo(culture.Key));

    var selectCulture = new CultureInfo(builder.Configuration["DefaultLanguage"]);

    builder.Services.Configure<RequestLocalizationOptions>(options =>
    {
        options.DefaultRequestCulture = new RequestCulture(selectCulture);
        options.SupportedCultures = supportedCultures;
        options.SupportedUICultures = supportedCultures;
        options.RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new QueryStringRequestCultureProvider(),
                    new CookieRequestCultureProvider()
                };
    });
}