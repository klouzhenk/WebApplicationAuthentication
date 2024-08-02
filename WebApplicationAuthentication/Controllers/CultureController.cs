using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApplicationAuthentication.Controllers
{
    [Route("[controller]/[action]")]
    public class CultureController : Controller
    {
        private readonly ILogger<CultureController> _logger;

        public CultureController(ILogger<CultureController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Set(string culture, string redirectUri)
        {
            _logger.LogInformation("Set action called with culture: {Culture}, redirectUri: {RedirectUri}", culture, redirectUri);

            if (string.IsNullOrEmpty(redirectUri))
            {
                _logger.LogWarning("RedirectUri is null or empty");
                return BadRequest("RedirectUri cannot be null or empty");
            }

            if (culture != null)
            {
                try
                {
                    var requestCulture = new RequestCulture(culture, culture);
                    var cookieName = CookieRequestCultureProvider.DefaultCookieName;
                    var cookieValue = CookieRequestCultureProvider.MakeCookieValue(requestCulture);

                    HttpContext.Response.Cookies.Append(cookieName, cookieValue);
                    _logger.LogInformation("Culture cookie set successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error setting culture cookie");
                    return StatusCode(500, "Internal server error");
                }
            }

            return LocalRedirect(redirectUri);
        }
    }
}
