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
                await JSRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
                await Task.Delay(2000);
                NavigationManager.NavigateTo("/", true);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"JavaScript interop failed: {ex.Message}");
            }
        }
    }
}