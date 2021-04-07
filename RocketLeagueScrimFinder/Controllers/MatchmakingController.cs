using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RocketLeagueScrimFinder.Data;
using RocketLeagueScrimFinder.Extensions;
using RocketLeagueScrimFinder.Services;

namespace RocketLeagueScrimFinder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MatchmakingController : ControllerBase
    {
        private MatchmakingService _matchmakingService;
        private TrackerService _trackerService;

        public MatchmakingController(MatchmakingService matchmakingService, TrackerService trackerService)
        {
            _matchmakingService = matchmakingService;
            _trackerService = trackerService;
        }

        [Route("updateMatchmakingPreference")]
        [HttpPost]
        public IActionResult UpdateMatchmakingPreference([FromBody] int matchMakingPreference)
        {
            var steamId = UserExtensions.GetSteamId(this.User);
            _matchmakingService.UpdatePreference(steamId, matchMakingPreference);
            return Ok();
        }

        [Route("updateServerSelection")]
        [HttpPost]
        public IActionResult UpdateServerSelection([FromBody] IEnumerable<int> selectedServers)
        {
            var steamId = UserExtensions.GetSteamId(this.User);
            _matchmakingService.UpdateServerSelection(steamId, selectedServers);
            return Ok();
        }

        [Route("updateCollegiateChecked")]
        [HttpPost]
        public IActionResult UpdateCollegiateChecked([FromBody] bool collegiateChecked)
        {
            var steamId = UserExtensions.GetSteamId(this.User);
            _matchmakingService.UpdateCollegiateSelected(steamId, collegiateChecked);
            return Ok();
        }

        [Route("isQueued")]
        [HttpGet]
        public bool IsQueued()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                var steamId = UserExtensions.GetSteamId(this.User);
                return _matchmakingService.GetIsQueued(steamId);
            }
            return false;
        }

        [Route("existingLobby")]
        [HttpGet]
        public MatchmakingData GetExistingLobbyInfo()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                var steamId = UserExtensions.GetSteamId(this.User);
                return _matchmakingService.GetExistingLobbyInfo(steamId);
            }
            return null;
        }

        [Route("startSearch")]
        [HttpPost]
        public IActionResult StartSearch([FromBody] UserInfo userInfo)
        {
            var steamId = UserExtensions.GetSteamId(this.User);
            if (string.IsNullOrWhiteSpace(steamId) || !userInfo.Servers.Any())
            {
                return BadRequest();
            }
            var mmr = _trackerService.GetMmr(steamId).Result;

            userInfo.SteamId = steamId;
            userInfo.Mmr = mmr;
            _matchmakingService.StartSearch(userInfo);
            return Ok();
        }

        [Route("cancelSearch")]
        [HttpPost]
        public IActionResult CancelSearch()
        {
            var steamId = UserExtensions.GetSteamId(this.User);
            _matchmakingService.CancelSearch(steamId);
            return Ok();
        }

        [Route("leaveLobby")]
        [HttpPost]
        public IActionResult LeaveLobby()
        {
            var steamId = UserExtensions.GetSteamId(this.User);
            _matchmakingService.LeaveLobby(steamId);
            return Ok();
        }

        [Route("queuedPlayers")]
        [HttpGet]
        public IEnumerable<UserInfo> RetrieveQueuedPlayers()
        {
            return _matchmakingService.RetrieveQueuedPlayers();
        }

        [Route("acceptMatch")]
        [HttpPost]
        public IActionResult AcceptMatch()
        {
            var steamId = UserExtensions.GetSteamId(this.User);
            _matchmakingService.AcceptMatch(steamId);
            return Ok();
        }

        [Route("declineMatch")]
        [HttpPost]
        public IActionResult DeclineMatch()
        {
            var steamId = UserExtensions.GetSteamId(this.User);
            _matchmakingService.DeclineMatch(steamId);
            return Ok();
        }
    }
}