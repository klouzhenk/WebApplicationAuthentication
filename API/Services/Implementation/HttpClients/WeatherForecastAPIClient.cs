using API.Entities;
using API.Services.Interfaces.HttpClients;

namespace API.Services.Implementation.HttpClients
{
    public class WeatherForecastAPIClient : IWeatherForecastAPIClient
    {
        private readonly HttpClient _client;

        public WeatherForecastAPIClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<Forecast[]> GetForecastsAsync(string idTown, string day)
        {
            var url = $"/WeatherForecast?idTown={idTown}&day={day}";
            return await _client.GetFromJsonAsync<Forecast[]>(url);
        }
    }
}
