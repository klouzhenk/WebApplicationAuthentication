using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using WebApplicationShared.Helpers;
using WebApplicationShared;

using Microsoft.Extensions.Logging;

using API.Services.Implementation.HttpClients;
using API.Services.Interfaces.HttpClients;
using API.Services.Implementation.DataServices;
using API.Services.Interfaces.DataServices;
using Serilog;

namespace ProjectMaui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });



            // Add services to the container.
            builder.Services.AddLocalization();
            builder.Services.AddControllers();
            //builder.Services.AddRazorPages();
            //builder.Services.AddRazorComponents().AddInteractiveServerComponents();
            builder.Services.AddHttpContextAccessor();
            //builder.Services.AddControllers().AddMvcOptions(x => x.CacheProfiles.Clear());

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

            //builder.Services.AddAuthenticationCore();
            //builder.Services.AddAuthorizationCore();

            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
            //builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<CustomAuthStateProvider>());
            builder.Services.AddSingleton<JwtSecurityTokenHandler>();
            builder.Services.AddScoped<DeleteAccountService>();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme);




#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
