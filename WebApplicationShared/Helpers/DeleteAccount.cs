using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using WebApplicationShared.Model;

namespace WebApplicationShared.Helpers
{
    public class DeleteAccountService
    {
        private readonly HttpClient _http;
        private readonly NavigationManager _navigationManager;
        private readonly CustomAuthStateProvider _authenticationStateProvider;
        private readonly IJSRuntime _jsRuntime;

        public DeleteAccountService(HttpClient http, NavigationManager navigationManager, AuthenticationStateProvider authenticationStateProvider, IJSRuntime jsRuntime)
        {
            _http = http;
            _navigationManager = navigationManager;
            _authenticationStateProvider = (CustomAuthStateProvider)authenticationStateProvider;
            _jsRuntime = jsRuntime;
        }

        public async Task DeleteUserAccountAsync()
        {
            var response = await _http.DeleteAsync("auth/delete-self");
            if (response.IsSuccessStatusCode)
            {
                // Logout the user after account deletion
                _authenticationStateProvider.MarkUserAsLoggedOut();
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken"); // Видаляємо токен з localStorage
                _navigationManager.NavigateTo("/");
            }
            else
            {
                // Обробка помилок
                Console.WriteLine("Error deleting account.");
            }
        }
    }
}
