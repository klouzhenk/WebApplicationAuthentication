using Microsoft.AspNetCore.Mvc;

namespace WebApplicationAuthentication.Controllers
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
            var forecasts = _forecastDbContext.Forecasts
                .Where(u => u.IdTown == idTown && u.Day == day).ToArray();
            return forecasts;
        }
    }
}
