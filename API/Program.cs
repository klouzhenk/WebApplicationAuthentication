using Microsoft.EntityFrameworkCore;
using API.Models;
using API.Services.Interfaces.HttpClients;
using API.Services.Implementation.HttpClients;
using API.Services.Interfaces.DataServices;
using API.Services.Implementation.DataServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp",
        builder =>
        {
            builder.WithOrigins("https://localhost:7147") // URL Blazor додатка
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddDbContext<ForecastDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));



builder.Services.AddHttpClient<IUserAPIClient, UserAPIClient>( configureClient =>
{
    configureClient.BaseAddress = new Uri("https://localhost:7267");
});
builder.Services.AddTransient<IUserDataService, UserDataService>();

builder.Services.AddHttpClient<IWeatherForecastAPIClient, WeatherForecastAPIClient>(configureClient =>
{
    configureClient.BaseAddress = new Uri("https://localhost:7267");
});
builder.Services.AddTransient<IWeatherForecastDataService, WeatherForecastDataService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();  // Ensure this is called before UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();
