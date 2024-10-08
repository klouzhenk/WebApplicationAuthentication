using API.Entities;

namespace API.Services.Interfaces.DataServices;

public interface IWeatherForecastDataService
{
    Task<Forecast[]> GetForecastsAsync(string idTown, string day);
}
