using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using API.Services.Interfaces.DataServices;
using WebApplicationShared.Model;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace WebApplicationShared.Components.Pages
{
    public partial class LoginPage : ComponentBase
    {
        // Parameters and Injected services
        [Parameter] public AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject] private IUserDataService DataService { get; set; }
        [Inject] private IJSRuntime JSRuntime { get; set; }
        [Inject] public ProtectedLocalStorage Storage { get; set; }

        public UserModel User { get; set; } = new();
        public RegisterRequest Register { get; set; } = new();
        public UserModel UserInfo { get; set; } = new();

        public string ErrorMessage { get; set; } = string.Empty;
        public bool IsErrorVisible { get; set; } = false;
        public bool IsSignUpHidden { get; set; } = true;

        private string _authToken;
        private bool _isAuthenticated = true;

        // Toggle sign-up form visibility
        public void ChangeHiding()
        {
            IsSignUpHidden = !IsSignUpHidden;
            StateHasChanged();
        }

        // Authenticate the user
        public async Task Authenticate()
        {
            var response = await DataService.LoginUserAsync(User.Name, User.Password);

            if (!response.IsSuccessStatusCode)
            {
                await _setErrorMessage(response);
                return;
            }

            var responseContent = await response.Content.ReadFromJsonAsync<JwtResponse>();
            if (responseContent != null)
            {
                _authToken = responseContent.Token;
                await JSRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", _authToken);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, User.Name)
                };
                var identity = new ClaimsIdentity(claims, "Authentication");
                var principal = new ClaimsPrincipal(identity);

                var authStateProvider = (CustomAuthStateProvider)AuthenticationStateProvider;
                await authStateProvider.MarkUserAsAuthenticated(principal);

                var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
                if (authState.User.Identity.IsAuthenticated)
                {
                    UserInfo = UserModel.GetUserInfoFromClaims(authState.User);
                }
            }
            else
            {
                await ShowError("Invalid response from server");
            }
        }

        // Handle registration
        public async Task Registration()
        {
            try
            {
                var response = await DataService.RegisterUserAsync(Register.Username, Register.Password, Register.Role);

                if (response.IsSuccessStatusCode)
                {
                    ChangeHiding();
                }
                else
                {
                    await _setErrorMessage(response);
                }
            }
            catch (Exception ex)
            {
                await ShowError($"An error occurred: {ex.Message}");
            }
        }

        // Set error message and display it
        private async Task _setErrorMessage(HttpResponseMessage response)
        {
            var errorResponse = await response.Content.ReadAsStringAsync();
            var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(errorResponse);

            if (problemDetails != null)
            {
                await ShowError(problemDetails.Detail);
            }
            else
            {
                await ShowError("No error details were provided.");
            }
        }

        // Display error message
        private async Task ShowError(string errorMsg)
        {
            ErrorMessage = errorMsg;
            IsErrorVisible = true;
            StateHasChanged();
            await Task.Delay(6000);
            IsErrorVisible = false;
            StateHasChanged();
        }
    }
}
