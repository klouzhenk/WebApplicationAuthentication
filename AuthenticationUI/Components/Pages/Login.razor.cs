using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using AuthenticationUI.Components.Pages;
using Microsoft.JSInterop;

namespace AuthenticationUI
{
    public partial class LoginPage : ComponentBase
    {
        // public fields
        [CascadingParameter] public HttpContext HttpContext { get; set; }
        [SupplyParameterFromForm] public UserModel User { get; set; } = new();

        // private fields
        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject] private HttpClient Http { get; set; }
        [Inject] private NavigationManager Navigation { get; set; }
        [Inject] private IJSRuntime JSRuntime { get; set; }

        private string _authToken;
        public string _errorMessage { get; set; }

        public async Task Authenticate()
        {
            try
            {
                var response = await Http.PostAsJsonAsync("/Auth/login", new { Username = User.Name, Password = User.Password });

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadFromJsonAsync<JwtResponse>();
                    if (responseContent != null)
                    {
                        _authToken = responseContent.Token;

                        // Зберегти токен у cookie
                        var cookieOptions = new CookieOptions { Expires = DateTime.UtcNow.AddHours(0.05) };
                        HttpContext.Response.Cookies.Append("auth_token", _authToken, cookieOptions);

                        var claims = new List<Claim> { new Claim(ClaimTypes.Name, User.Name) };
                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);

                        ((CustomAuthStateProvider)AuthenticationStateProvider).MarkUserAsAuthenticated(principal);
                        await HttpContext.SignInAsync(principal);
                    }
                    else
                    {
                        _errorMessage = "Invalid response from server";
                    }
                }
                else
                {
                    _errorMessage = "Invalid username or password";
                }
            }
            catch (Exception ex)
            {
                _errorMessage = $"An error occurred: {ex.Message}";
            }
        }
    }
}