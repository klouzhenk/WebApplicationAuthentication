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

namespace BlazorMaui
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

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            // Add services similar to Blazor Web App
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddLocalization();
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
            builder.Services.AddScoped<BlazorMaui.CustomAuthStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider, BlazorMaui.CustomAuthStateProvider>();
            builder.Services.AddSingleton<JwtSecurityTokenHandler>();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => builder.Configuration.Bind("JwtSettings", options));

            builder.Services.AddAuthorization();

            // Реєстрація ChatService
            builder.Services.AddSingleton<ChatService>();

            return builder.Build();
        }
    }
}
