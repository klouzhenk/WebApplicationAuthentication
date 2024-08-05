using WebApplicationAuthentication.Entities;

namespace WebApplicationAuthentication.Services.Interfaces
{
    public interface IWeatherForecastDataService
    {
        Task<Forecast[]> GetForecastsAsync();
    }
}
