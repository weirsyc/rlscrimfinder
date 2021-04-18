using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RocketLeagueScrimFinder.Data;
using RocketLeagueScrimFinder.Extensions;
using RocketLeagueScrimFinder.Models;
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
        private ScrimFinderContext _dbContext;

        public UserController(TrackerService trackerService, SteamService steamService, ScrimFinderContext dbContext)
        {
            _trackerService = trackerService;
            _steamService = steamService;
            _dbContext = dbContext;
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

        [Route("getSettings")]
        [HttpGet]
        [Authorize]
        public UserSettings GetUserSettings()
        {
            var steamId = UserExtensions.GetSteamId(this.User);
            return _dbContext.UserSettings.FirstOrDefault(u => u.SteamId == steamId);
        }

        [Route("updateSettings")]
        [HttpPost]
        [Authorize]
        public void UpdateSettings([FromBody] UserSettings userSettings)
        {
            var existingSettings = _dbContext.UserSettings.FirstOrDefault(u => u.SteamId == userSettings.SteamId);
            if (existingSettings == null)
            {
                _dbContext.UserSettings.Add(userSettings);
            }
            else
            {
                existingSettings.DiscordId = userSettings.DiscordId;
                _dbContext.UserSettings.Update(existingSettings);
            }
            _dbContext.SaveChanges();
        }

    }
}
