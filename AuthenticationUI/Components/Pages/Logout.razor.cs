using Microsoft.AspNetCore.Components; 
using Microsoft.JSInterop; 
using System; 
using System.Threading.Tasks;

namespace AuthenticationUI 
{
    public partial class LogoutPage : ComponentBase // Клас LogoutPage, який є частковим та успадковується від ComponentBase
    {
        [Inject] private HttpClient Http { get; set; } 
        [Inject] private NavigationManager NavigationManager { get; set; } 
        [Inject] private IJSRuntime JSRuntime { get; set; } 

        public bool showModal = false; 

        public void ShowLogoutConfirmation()
        {
            showModal = true; 
        }

        // Метод, який викликається при підтвердженні виходу
        public async Task OnLogoutConfirmed(bool isConfirmed)
        {
            if (isConfirmed) 
            {
                try
                {
                    // Видалення токена аутентифікації з локального сховища
                    await JSRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
                    // Перенаправлення на головну сторінку з примусовим перезавантаженням
                    NavigationManager.NavigateTo("/", true);
                }
                catch (Exception ex) 
                {
                    // Виведення повідомлення про помилку у консоль
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
