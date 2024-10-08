using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using API.Services.Interfaces.DataServices;
using WebApplicationShared.Model;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using WebApplicationShared;
using Serilog;
using API.Controllers;

namespace BlazorMaui.Components.Pages
{
	public partial class LoginPage : ComponentBase
    {
        // public fields
        [CascadingParameter] public HttpContext HttpContext { get; set; }
        [SupplyParameterFromForm] public UserModel User { get; set; } = new();
        [SupplyParameterFromForm] public RegisterRequest Register { get; set; } = new();
        public UserModel UserInfo { get; set; } = new();

        // private fields
        [Inject] public CustomAuthStateProvider AuthenticationStateProvider { get; set; }
        [Inject] private IUserDataService DataService { get; set; }
        [Inject] private IJSRuntime JSRuntime { get; set; }

        public string ErrorMessage = string.Empty;
        public bool IsErrorVisible = false;
        public bool IsSignUpHidden = true;

        private string _authToken;
        private bool _isAuthenticated = true;

        public void ChangeHiding()
        {
            IsSignUpHidden = IsSignUpHidden ? false : true;
            return;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            Log.Information("\n\nStart AUTHENTICATION ---------------------------\n");
            if (firstRender)
            {
                AuthenticationStateProvider.CheckAuthenticationAfterRendering();
            }
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

            if (!authState.User.Identity.IsAuthenticated) { return; }

            var userClaims = authState.User.Claims;
            UserInfo = UserModel.GetUserInfoFromClaims(authState.User);

            Log.Information("\n\n Finish AUTHENTICATION ---------------------------\n");

            StateHasChanged();
        }

        public async Task Authenticate()
        {
            var response = await DataService.LoginUserAsync(User.Name, User.Password);

            if (!response.IsSuccessStatusCode)
            {
                _setErrorMessage(response);
                return;
            }

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

                var authState = await AuthenticationStateProvider.MarkUserAsAuthenticated(principal);
                if (!authState.User.Identity.IsAuthenticated) { return; }
                var userClaims = authState.User.Claims;
                UserInfo = UserModel.GetUserInfoFromClaims(authState.User);

                StateHasChanged();
            }
            else
            {
                ShowError("Invalid response from server");
            }

            StateHasChanged();
        }

        private async void _setErrorMessage(HttpResponseMessage response)
        {
            var errorResponse = await response.Content.ReadAsStringAsync();
            var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(errorResponse);

            if (problemDetails != null) { ShowError(problemDetails.Detail); }
            else { ShowError("No error details were provided."); }

            StateHasChanged();
        }

        public async Task Registration()
        {
            try
            {
                var response = await DataService.RegisterUserAsync(Register.Username, Register.Password, Register.Role);

                if (response.IsSuccessStatusCode) { ChangeHiding(); }
                else { _setErrorMessage(response); }
            }
            catch (Exception ex) { ShowError($"An error occurred: {ex.Message}"); }

            StateHasChanged();
        }

        private async Task ShowError(string errorMsg)
        {
            ErrorMessage = errorMsg;
            IsErrorVisible = true;
            await Task.Delay(6000);
            IsErrorVisible = false;
            StateHasChanged();
        }
    }
}
