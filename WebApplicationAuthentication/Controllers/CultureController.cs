using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplicationAuthentication.Controllers
{
    [Route("[controller]/[action]")]
    public class CultureController : Controller
    {
        [HttpGet]
        public IActionResult Test()
        {
            return Content("Test route is working");
        }

        [HttpGet]
        public IActionResult Set(string culture, string redirectUri)
        {
            if (string.IsNullOrEmpty(culture))
            {
                return BadRequest("Culture cannot be null or empty");
            }

            if (string.IsNullOrEmpty(redirectUri))
            {
                return BadRequest("RedirectUri cannot be null or empty");
            }

            // Add logic to handle culture and redirectUri
            var requestCulture = new RequestCulture(culture, culture);
            var cookieName = CookieRequestCultureProvider.DefaultCookieName;
            var cookieValue = CookieRequestCultureProvider.MakeCookieValue(requestCulture);

            HttpContext.Response.Cookies.Append(cookieName, cookieValue);

            return LocalRedirect(redirectUri);

        }

    }
}
