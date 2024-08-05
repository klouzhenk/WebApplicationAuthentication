using WebApplicationAuthentication.Entities;
using WebApplicationAuthentication.Services.Interfaces.DataServices;
using WebApplicationAuthentication.Services.Interfaces.HttpClients;

namespace WebApplicationAuthentication.Services.Implementation.DataServices
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
