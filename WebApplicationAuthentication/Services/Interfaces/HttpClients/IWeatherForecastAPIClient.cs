using WebApplicationAuthentication.Entities;

namespace WebApplicationAuthentication.Services.Interfaces.HttpClients
{
    public interface IWeatherForecastAPIClient
    {
        Task<Forecast[]> GetForecastsAsync(string idTown, string day);
    }
}
