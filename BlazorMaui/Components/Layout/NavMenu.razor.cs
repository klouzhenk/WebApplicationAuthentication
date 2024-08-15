using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorMaui.Components.Layout
{
    public class NavMenuPage : ComponentBase
    {
        public bool IsNavListVisible = false;
        public bool showModal = false;
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private IJSRuntime JSRuntime { get; set; }

        public void ChangeNavListVisibility()
        {
            IsNavListVisible = IsNavListVisible ? false : true;
        }

        public void ShowLogoutConfirmation()
        {
            showModal = true;
        }

        public async Task OnLogoutConfirmed(bool isConfirmed)
        {
            if (isConfirmed)
            {
                try
                {
                    await JSRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
                    NavigationManager.NavigateTo("/", true);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"JavaScript interop failed: {ex.Message}");
                }
            }
            showModal = false;
            ChangeNavListVisibility();
        }
        public void OnCancelLogout()
        {
            showModal = false;
            ChangeNavListVisibility();
        }
    }
}
