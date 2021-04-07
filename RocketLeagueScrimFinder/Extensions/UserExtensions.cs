using System.Linq;
using System.Security.Claims;

namespace RocketLeagueScrimFinder.Extensions
{
    public static class UserExtensions
    {
        public static string GetSteamId(ClaimsPrincipal user)
        {
            var openIdUrl = user.Claims?.FirstOrDefault()?.Value;
            return openIdUrl.Replace("https://steamcommunity.com/openid/id/", string.Empty);
        }
    }
}
