using WebApplicationAuthentication.Entities;

namespace WebApplicationAuthentication.Services.Interfaces.DataServices
{
    public interface IWeatherForecastDataService
    {
        Task<Forecast[]> GetForecastsAsync(string idTown, string day);
    }
}
