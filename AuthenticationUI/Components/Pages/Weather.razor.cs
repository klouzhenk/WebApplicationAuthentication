using Microsoft.AspNetCore.Components;
using AuthenticationUI.Components.Pages.Model;
using Microsoft.AspNetCore.Authorization;


namespace AuthenticationUI
{
    [Authorize]
    public partial class WeatherClass : ComponentBase
    {
        public WeatherForecast[]? forecasts;
        [Inject] private HttpClient Http { get; set; }
        [Inject] private CustomAuthStateProvider AuthenticationStateProvider { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                ((CustomAuthStateProvider)AuthenticationStateProvider).CheckAuthenticationAfterRendering();
            }
        }

        //protected override async Task OnInitializedAsync()
        //{
        //    var authState = await CustomAuthStateProvider.GetAuthenticationStateAsync();
        //    if (!authState.User.Identity.IsAuthenticated)
        //    {
        //        // Перенаправлення на сторінку входу або повідомлення про помилку
        //        NavigationManager.NavigateTo("/");
        //        return;
        //    }

        //    try
        //    {
        //        forecasts = await Http.GetFromJsonAsync<WeatherForecast[]>("WeatherForecast");
        //    }
        //    catch (HttpRequestException ex)
        //    {
        //        // Логування помилки
        //        Console.Error.WriteLine($"Failed to fetch weather data: {ex.Message}");
        //    }
        //}
    }
}
