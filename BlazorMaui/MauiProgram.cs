using Microsoft.Extensions.Logging;
using BlazorMaui.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using WebApplicationShared.Helpers;
using WebApplicationShared;
using Serilog;
using Microsoft.Extensions.Configuration;

using API.Services.Implementation.HttpClients;
using API.Services.Interfaces.HttpClients;
using API.Services.Implementation.DataServices;
using API.Services.Interfaces.DataServices;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Security.Claims;

namespace BlazorMaui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            ConfigureServices(builder);
            return builder.Build();
        }

        private static void ConfigureServices(MauiAppBuilder builder)
        {
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            // Add necessary services
            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddLocalization();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            // Configure HttpClient and custom data services
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

            // Add authentication and authorization
            builder.Services.AddAuthenticationCore();
            builder.Services.AddScoped<BlazorMaui.CustomAuthStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<BlazorMaui.CustomAuthStateProvider>());

            builder.Services.AddAuthorizationCore();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => builder.Configuration.Bind("JwtSettings", options));

            // Register custom services
            builder.Services.AddSingleton<JwtSecurityTokenHandler>();
            builder.Services.AddSingleton<ChatService>();
            builder.Services.AddSingleton<UserModel>();
        }
    }
}
