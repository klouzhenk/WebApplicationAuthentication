﻿using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using API.Services.Interfaces.DataServices;
using AuthenticationUI.Model;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AuthenticationUI.Components.Pages
{
    public partial class LoginPage : ComponentBase
    {
        // public fields
        [CascadingParameter] public HttpContext HttpContext { get; set; }
        [SupplyParameterFromForm] public UserModel User { get; set; } = new();
        public UserModel UserInfo { get; set; } = new();
        [SupplyParameterFromForm] public RegisterRequest Register { get; set; } = new();

        // private fields
        [Inject] private CustomAuthStateProvider AuthenticationStateProvider { get; set; }
        [Inject] private IUserDataService DataService { get; set; }
        [Inject] private IJSRuntime JSRuntime { get; set; }
        [Inject] public ProtectedLocalStorage storage { get; set; }

        public string ErrorMessage;
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
            if (firstRender)
            {
                AuthenticationStateProvider.CheckAuthenticationAfterRendering();
            }
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

            if (!authState.User.Identity.IsAuthenticated) { return; }

            var userClaims = authState.User.Claims;
            UserInfo = UserModel.GetUserInfoFromClaims(authState.User);
        }

        public async Task Authenticate()
        {
            try
            {
                var response = await DataService.LoginUserAsync(User.Name, User.Password);

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

                        var authState = await AuthenticationStateProvider.MarkUserAsAuthenticated(principal);
                        if (!authState.User.Identity.IsAuthenticated) { return; }
                        var userClaims = authState.User.Claims;
                        UserInfo = UserModel.GetUserInfoFromClaims(authState.User);
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

            StateHasChanged();
        }


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
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(errorResponse);

                    if (problemDetails != null)
                    {
                        ErrorMessage = $"Signing up wasn't successful: {problemDetails.Detail}";
                    }
                    else
                    {
                        ErrorMessage = "Signing up wasn't successful and no error details were provided.";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
            }

            StateHasChanged();
        }
    }
}
