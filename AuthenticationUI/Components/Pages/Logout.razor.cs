using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace AuthenticationUI
{
    public partial class Logout : ComponentBase
    {
        [Inject] private HttpClient Http { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private IJSRuntime JSRuntime { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                // Видалення токена з localStorage
                await JSRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
                // Delay
                await Task.Delay(2000);
                // Перенаправлення на сторінку входу 
                NavigationManager.NavigateTo("/", true);
            }
            catch (Exception ex)
            {
                // Логування помилки (можливо, на консоль або в якийсь інший спосіб)
                Console.Error.WriteLine($"JavaScript interop failed: {ex.Message}");
            }
        }
    }
}