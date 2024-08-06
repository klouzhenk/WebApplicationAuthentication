using API.Entities;
using API.Services.Interfaces.DataServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace AuthenticationUI.Components.Pages
{
    [Authorize]
    public partial class WeatherPage : ComponentBase
    {
        public Forecast[]? forecasts;
        [Inject] private IWeatherForecastDataService DataService { get; set; }
        [Inject] private CustomAuthStateProvider CustomAuthStateProvider { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                CustomAuthStateProvider.CheckAuthenticationAfterRendering();

                var authState = await CustomAuthStateProvider.GetAuthenticationStateAsync();

                if (!authState.User.Identity.IsAuthenticated)
                {
                    NavigationManager.NavigateTo("/");
                    return;
                }

                try
                {
                    var idTownClaim = authState.User.FindFirst("IdTown");
                    if (idTownClaim != null)
                    {
                        string idTown = idTownClaim.Value;
                        string day = DateTime.Today.ToString("dd.MM.yyyy");
                        //string day = "02.08.2024";
                        forecasts = await DataService.GetForecastsAsync(idTown, day);
                        var sortedForecasts = forecasts.Select(f => new WeatherForecast
                        {
                            Time = f.Time,
                            Temperature = f.Temperature,
                            State = f.State
                        }).ToArray();
                    }
                }
                catch (HttpRequestException ex)
                {
                    Console.Error.WriteLine($"Failed to fetch weather data: {ex.Message}");
                }

                StateHasChanged();
            }
        }
    }
}
