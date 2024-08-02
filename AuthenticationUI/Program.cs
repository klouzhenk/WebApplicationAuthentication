using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;
using AuthenticationUI.Components;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// ������ ������� �� ����������
builder.Services.AddRazorPages();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers().AddMvcOptions(x => x.CacheProfiles.Clear());

builder.Services.AddAuthenticationCore();
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddSingleton<JwtSecurityTokenHandler>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => builder.Configuration.Bind("JwtSettings", options));

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

// ������ ����������
//builder.Services.AddLocalization(options => options.ResourcesPath = "Locales");
AddLocalization(builder);

// ������������ HttpClient
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7267")
});

// ������������ ����������
var supportedCultures = new List<CultureInfo>();
var cultures = builder.Configuration.GetSection("Cultures").GetChildren().ToDictionary(x => x.Key, x => x.Value);
foreach (var culture in cultures)
    supportedCultures.Add(new CultureInfo(culture.Key));

var defaultCulture = new CultureInfo(builder.Configuration["DefaultLanguage"]);

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(defaultCulture);
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    options.RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new QueryStringRequestCultureProvider(),
        new CookieRequestCultureProvider()
    };
});

var app = builder.Build();

// ������������ ������� ������� ������
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
app.UseRequestLocalization(app.Services.GetService<IOptions<RequestLocalizationOptions>>()?.Value!);
//app.UseRequestLocalization(app.Services.GetService<IOptions<RequestLocalizationOptions>>()?.Value);
app.UseAntiforgery();

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