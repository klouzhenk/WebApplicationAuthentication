using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AuthenticationUI.Components.Pages.Model;

namespace AuthenticationUI
{
    [Authorize]
    public partial class WeatherClass : ComponentBase
    {
        public WeatherForecast[]? forecasts;
        [Inject] private HttpClient Http { get; set; }
        [Inject] private CustomAuthStateProvider CustomAuthStateProvider { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            if (!authState.User.Identity.IsAuthenticated)
            {
                // Перенаправлення на сторінку входу
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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                CustomAuthStateProvider.CheckAuthenticationAfterRendering();
            }

            var authState = await CustomAuthStateProvider.GetAuthenticationStateAsync();
            if (!authState.User.Identity.IsAuthenticated)
            {
                NavigationManager.NavigateTo("/");
                return;
            }