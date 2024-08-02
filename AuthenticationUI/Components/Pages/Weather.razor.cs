using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace AuthenticationUI.Components.Pages
{

    [Authorize]
    public partial class WeatherPage : ComponentBase
    {
        public WeatherForecast[]? forecasts;
        [Inject] private HttpClient Http { get; set; }
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

                        var url = $"WeatherForecast?idTown={idTown}&day={day}";
                        forecasts = await Http.GetFromJsonAsync<WeatherForecast[]>(url);
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
