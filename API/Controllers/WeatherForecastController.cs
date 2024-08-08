using API.Entities;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ForecastDbContext _forecastDbContext;

        public WeatherForecastController(ForecastDbContext forecastDbContext)
        {
            _forecastDbContext = forecastDbContext;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public Forecast[] Get([FromQuery] int idTown, [FromQuery] string day)
        {
            Log.Information("\n\nStart getting weather forecasts ---------------------------\n");
            var forecasts = _forecastDbContext.Forecasts
                .Where(f => f.IdTown == idTown && f.Day == day).ToArray();
            Log.Information("\n\nFinish getting weather forecasts ---------------------------\n");
            return forecasts;
        }
    }
}