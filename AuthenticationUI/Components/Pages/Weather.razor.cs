using Microsoft.AspNetCore.Components;
using AuthenticationUI.Components.Pages.Model;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;


namespace AuthenticationUI
{
    public partial class WeatherClass : ComponentBase
    {
        public WeatherForecast[]? forecasts;
        [Inject] private HttpClient Http { get; set; } = default!;
        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            if (!authState.User.Identity.IsAuthenticated)
            {
                // Перенаправлення на сторінку входу або повідомлення про помилку
                NavigationManager.NavigateTo("/");
                return;
            }

            try
            {
                forecasts = await Http.GetFromJsonAsync<WeatherForecast[]>("WeatherForecast");
            }
            catch (HttpRequestException ex)
            {
                // Логування помилки
                Console.Error.WriteLine($"Failed to fetch weather data: {ex.Message}");
            }
        }
    }
}
