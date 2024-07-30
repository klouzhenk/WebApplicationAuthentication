using Microsoft.AspNetCore.Components.Authorization;

using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using AuthenticationUI.Components.Pages;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using AuthenticationUI.Components.Pages.Model;

namespace AuthenticationUI
{
    public partial class LoginPage : ComponentBase
    {
        // public fields
        [CascadingParameter] public HttpContext HttpContext { get; set; }
        [SupplyParameterFromForm] public UserModel User { get; set; } = new();
        [SupplyParameterFromForm] public RegisterRequest Register { get; set; } = new();

        // private fields
        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject] private HttpClient Http { get; set; }
        [Inject] private IJSRuntime JSRuntime { get; set; }
        [Inject] public ProtectedLocalStorage storage { get; set; }

        public string ErrorMessage;
        public bool IsSignUpHidden = false;


        private string _authToken;
        private bool _isAuthenticated = false;

        public void ChangeHiding()
        {
            IsSignUpHidden = IsSignUpHidden ? false : true;
            return;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                ((CustomAuthStateProvider)AuthenticationStateProvider).CheckAuthenticationAfterRendering();
            }
        }

        public async Task Authenticate()
        {
            try
            {
                var response = await Http.PostAsJsonAsync("/Auth/login", new { Username = User.Name, User.Password });

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadFromJsonAsync<JwtResponse>();
                    if (responseContent != null)
                    {
                        _authToken = responseContent.Token;
                        _isAuthenticated = true;
                        await JSRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", _authToken);
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, User.Name)
                        };
                        var identity = new ClaimsIdentity(claims, "Authentication");
                        var principal = new ClaimsPrincipal(identity);

                        ((CustomAuthStateProvider)AuthenticationStateProvider).MarkUserAsAuthenticated(principal);
                    }
                    else
                    {
                        ErrorMessage = "Invalid response from server";
                    }
                }
                else
                {
                    ErrorMessage = "Invalid username or password";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
            }
        }

        public async Task Registration()
        {
            try
            {
                var response = await Http.PostAsJsonAsync("/Auth/register", new { Register.Username, Register.Password, Register.Role });

                if (response.IsSuccessStatusCode)
                {
                    ChangeHiding();
                    Task.Delay(1000);
                }
                else
                {
                    ErrorMessage = "Singing up wasn't successful...";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
            }
        }
    }
}