using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public CustomAuthStateProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var cookieToken = httpContext?.Request.Cookies["auth_token"];
        ClaimsPrincipal user = new ClaimsPrincipal(new ClaimsIdentity());

        if (!string.IsNullOrEmpty(cookieToken))
        {
            try
            {
                //string correctTokenForm = cookieToken.Replace("-\0", ".");
                var token = _tokenHandler.ReadJwtToken(cookieToken);
                var claims = token.Claims;
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                user = new ClaimsPrincipal(identity);
            }
            catch (Exception)
            {
                user = new ClaimsPrincipal(new ClaimsIdentity());       // юзер - неавтифікований
            }
        }

        return Task.FromResult(new AuthenticationState(user));
    }

    public void MarkUserAsAuthenticated(ClaimsPrincipal user)
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public void MarkUserAsLoggedOut()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }
}