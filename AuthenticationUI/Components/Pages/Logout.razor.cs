using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;


namespace AuthenticationUI
{
    public partial class LogoutClass : ComponentBase
    {
        [Inject] private HttpClient Http { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private IJSRuntime JSRuntime { get; set; }
        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        protected async Task LogoutAsync()
        {
            try
            {
                // Видалення токена з localStorage
                await JSRuntime.InvokeVoidAsync("localStorage.removeItem", "auth_token");
                // Delay
                await Task.Delay(1000);
                // Перенаправлення на сторінку входу 
                NavigationManager.NavigateTo("/", true);
                ((CustomAuthStateProvider)AuthenticationStateProvider).MarkUserAsLoggedOut();
            }
            catch (Exception ex)
            {
                // Логування помилки (можливо, на консоль або в якийсь інший спосіб)
                Console.Error.WriteLine($"JavaScript interop failed: {ex.Message}");
            }
        }
    }
}