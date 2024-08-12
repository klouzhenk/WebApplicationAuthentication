using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Localization;

namespace WebApplicationShared.Controllers
{
    [Route("[controller]/[action]")]
    public class CultureController : Controller
    {
        public IActionResult Set(string culture, string redirectUri)
        {
            if(culture != null)
            {
                var requestCulture = new RequestCulture(culture, culture);
                var cookieName = CookieRequestCultureProvider.DefaultCookieName;
                var cookieValue = CookieRequestCultureProvider.MakeCookieValue(requestCulture);

                HttpContext.Response.Cookies.Append(cookieName, cookieValue);
            }

            return LocalRedirect(redirectUri);
        }
    }
}
