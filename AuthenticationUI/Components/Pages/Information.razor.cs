using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using API.Entities;
using API.Services.Interfaces.DataServices;
using WebApplicationShared;
using Microsoft.AspNetCore.Components.Authorization;

namespace AuthenticationUI.Components.Pages
{
    public partial class InformationPage : ComponentBase
    {
        [Inject] private CustomAuthStateProvider CustomAuthStateProvider { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }

        public UserModel UserInfo { get; set; } = new();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                CustomAuthStateProvider.CheckAuthenticationAfterRendering();
            }
            var authState = await CustomAuthStateProvider.GetAuthenticationStateAsync();

            if (!authState.User.Identity.IsAuthenticated)
            {
                NavigationManager.NavigateTo("/");
                return;
            }

            var userClaims = authState.User.Claims;
            UserInfo = UserModel.GetUserInfoFromClaims(authState.User);
            StateHasChanged();
        }
    }
}
