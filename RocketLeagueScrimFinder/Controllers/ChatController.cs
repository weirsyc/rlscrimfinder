using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RocketLeagueScrimFinder.Extensions;
using RocketLeagueScrimFinder.Services;

namespace RocketLeagueScrimFinder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private MatchmakingService _matchmakingService;

        public ChatController(MatchmakingService matchmakingService)
        {
            _matchmakingService = matchmakingService;
        }

        [Route("sendMessage")]
        [HttpPost]
        public IActionResult SendMessage([FromBody] string message)
        {
            var steamId = UserExtensions.GetSteamId(this.User);
            _matchmakingService.SendMessage(steamId, message);
            return Ok();
        }        
    }
}