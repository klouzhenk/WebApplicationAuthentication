﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Localization;

namespace Mint.Software.EReceipt.WebUI.Controllers
{
    [Route("[controller]/[action]")]
    public class CultureController : Controller
    {
        public IActionResult SetCulture(string culture, string redirectUri)
        {
            if (culture != null)
            {
                HttpContext.Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(
                        new RequestCulture(culture)));
            }
            return LocalRedirect(redirectUri);
        }
    }
}
