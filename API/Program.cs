using Microsoft.EntityFrameworkCore;
using API.Services.Implementation.DataServices;
using API.Models;
using API.Services.Interfaces.HttpClients;
using API.Services.Interfaces.DataServices;
using API.Services.Implementation.HttpClients;
using API.Middleware;
using Serilog;
using SignalRChat.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Services.AddSignalR();
builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddDbContext<ForecastDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp",
        builder =>
        {
            builder.WithOrigins("https://172.19.100.148:7267/swagger/index.html") // URL Blazor
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

// set up dataservices
builder.Services.AddHttpClient<IUserAPIClient, UserAPIClient>( configureClient =>
{
    configureClient.BaseAddress = new Uri("https://172.19.100.148:7267/swagger/index.html");
});
builder.Services.AddTransient<IUserDataService, UserDataService>();

builder.Services.AddHttpClient<IWeatherForecastAPIClient, WeatherForecastAPIClient>(configureClient =>
{
    configureClient.BaseAddress = new Uri("https://172.19.100.148:7267/swagger/index.html");
});
builder.Services.AddTransient<IWeatherForecastDataService, WeatherForecastDataService>();


var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapHub<ChatHub>("/chatHub");

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowBlazorApp");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
