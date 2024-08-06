using API.Entities;
using API.Services.Interfaces.DataServices;
using API.Services.Interfaces.HttpClients;

namespace API.Services.Implementation.DataServices
{
    public class WeatherForecastDataService : IWeatherForecastDataService
    {
        private IWeatherForecastAPIClient _client;
        public WeatherForecastDataService(IWeatherForecastAPIClient client)
        {
            _client = client;
        }
        public async Task<Forecast[]> GetForecastsAsync(string idTown, string day)
        {
            return await _client.GetForecastsAsync(idTown, day);
        }
    }
}
