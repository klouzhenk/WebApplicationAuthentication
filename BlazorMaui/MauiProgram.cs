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
using System.Security.Cryptography.X509Certificates;
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




            //HttpClientHandler clientHandler = new HttpClientHandler();
            //clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            //// Pass the handler to httpclient(from you are calling api)
            //HttpClient client = new HttpClient(clientHandler);



            // Configure HttpClient and custom data services
            builder.Services.AddHttpClient<IUserAPIClient, UserAPIClient>(configureClient =>
            {
                configureClient.BaseAddress = new Uri("https://172.19.100.148:7267/swagger/index.html");
            }).ConfigurePrimaryHttpMessageHandler(() => {
                var handler = new HttpClientHandler();

                // Add your custom HTTP client options here
                //handler.ClientCertificates = new X509CertificateCollection();
                handler.UseDefaultCredentials = true;
                handler.ServerCertificateCustomValidationCallback =
                    (sender, cert, chain, sslPolicyErrors) => true;

                return handler;
            });
            builder.Services.AddTransient<IUserDataService, UserDataService>();

            builder.Services.AddHttpClient<IWeatherForecastAPIClient, WeatherForecastAPIClient>(configureClient =>
            {
                configureClient.BaseAddress = new Uri("https://172.19.100.148:7267/swagger/index.html");
            }).ConfigurePrimaryHttpMessageHandler(() => {
                var handler = new HttpClientHandler();

                // Add your custom HTTP client options here
                //handler.ClientCertificates = new X509CertificateCollection();
                handler.UseDefaultCredentials = true;
                handler.ServerCertificateCustomValidationCallback =
                    (sender, cert, chain, sslPolicyErrors) => true;

                return handler;
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
