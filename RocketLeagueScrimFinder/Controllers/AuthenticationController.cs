using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace RocketLeagueScrimFinder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
#if DEBUG
        private const string REDIRECT_URI = "/";
#else
        private const string REDIRECT_URI = "https://rlscrimfinder.com";
#endif


        [Route("signin")]
        [HttpPost]
        public IActionResult SignIn()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = REDIRECT_URI }, "Steam");
        }
        
        [Route("signout")]
        [HttpGet]
        public IActionResult SignOut()
        {
            return SignOut(new AuthenticationProperties { RedirectUri = REDIRECT_URI },
                CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}