using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace AuthenticationUI.Components.Layout
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
            StateHasChanged();
            return;
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
        }
        public void OnCancelLogout()
        {
            showModal = false;
        }
    }
}
