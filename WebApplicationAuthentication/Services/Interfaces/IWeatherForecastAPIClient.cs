using WebApplicationAuthentication.Entities;

namespace WebApplicationAuthentication.Services.Interfaces
{
    public interface IWeatherForecastAPIClient
    {
        Task<Forecast[]> GetForecastsAsync();
    }
}
