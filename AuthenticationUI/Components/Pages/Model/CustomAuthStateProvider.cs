using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Microsoft.JSInterop;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime _jsRuntime;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private bool _isAuthenticated = false;
    private bool _isAuthenticationInProgress = true;
    private ClaimsPrincipal _user = new ClaimsPrincipal(new ClaimsIdentity());

    public CustomAuthStateProvider(IJSRuntime jsRuntime, bool isAuthenticationInProgress = false)
    {
        _jsRuntime = jsRuntime;
        _tokenHandler = new JwtSecurityTokenHandler();
        _isAuthenticationInProgress = isAuthenticationInProgress;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        await LoadAuthTokenAsync();
        return new AuthenticationState(_user);
    }

    public void MarkUserAsAuthenticated(ClaimsPrincipal user)
    {
        _isAuthenticated = true;
        _user = user;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public void CheckAuthenticationAfterRendering()
    {
        _isAuthenticationInProgress = true;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public void MarkUserAsLoggedOut()
    {
        _isAuthenticated = false;
        _user = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_user)));
    }

    private async Task LoadAuthTokenAsync()
    {
        if (_isAuthenticationInProgress && _jsRuntime != null)
        {
            string lsToken = string.Empty;
            try { lsToken = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "auth_token"); }
            catch(Exception ex) { Console.Error.WriteLine(ex.Message); }

            if (!string.IsNullOrEmpty(lsToken))
            {
                try
                {
                    var token = _tokenHandler.ReadJwtToken(lsToken);
                    var claims = token.Claims;
                    var identity = new ClaimsIdentity(claims, "auth_type");
                    _user = new ClaimsPrincipal(identity);
                }
                catch (Exception) { _user = new ClaimsPrincipal(new ClaimsIdentity()); }
            }
            else
            {
                _user = new ClaimsPrincipal(new ClaimsIdentity());
            }

            _isAuthenticationInProgress = false;
        }
    }
}