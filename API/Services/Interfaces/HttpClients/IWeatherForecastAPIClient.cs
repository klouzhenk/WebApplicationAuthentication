using API.Entities;

namespace API.Services.Interfaces.HttpClients
{
    public interface IWeatherForecastAPIClient
    {
        Task<Forecast[]> GetForecastsAsync(string idTown, string day);
    }
}
