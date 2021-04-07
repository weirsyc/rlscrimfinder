using Microsoft.AspNetCore.Mvc;
using RocketLeagueScrimFinder.Data;
using RocketLeagueScrimFinder.Extensions;
using RocketLeagueScrimFinder.Services;
using System.Linq;

namespace RocketLeagueScrimFinder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private TrackerService _trackerService;
        private SteamService _steamService;
        public UserController(TrackerService trackerService, SteamService steamService)
        {
            _trackerService = trackerService;
            _steamService = steamService;
        }

        [Route("userinfo")]
        [HttpGet]
        public UserInfo GetUserInfo()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                var steamId = UserExtensions.GetSteamId(this.User);
                var steamInfo = _steamService.GetSteamInfo(steamId);
                var userName = steamInfo.FirstOrDefault(u => u.Name == "personaname")?.Value;
                var mmr = _trackerService.GetMmr(steamId).Result;
                return new UserInfo { SteamId = steamId, DisplayName = userName, Mmr = mmr};
            }
            return null;
        }
    }
}
