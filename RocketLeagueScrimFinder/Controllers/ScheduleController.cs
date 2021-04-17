using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RocketLeagueScrimFinder.Data;
using RocketLeagueScrimFinder.Extensions;
using RocketLeagueScrimFinder.Models;
using RocketLeagueScrimFinder.Services;
using RocketLeagueScrimFinder.ViewModels;

namespace RocketLeagueScrimFinder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ScheduleController : ControllerBase
    {
        private SchedulingService _schedulingService;
        private TrackerService _trackerService;
        private SteamService _steamService;
        private DiscordService _discordService;
        private ScrimFinderContext _dbContext;

        public ScheduleController(SchedulingService schedulingService, 
            TrackerService trackerService, SteamService steamService,
            DiscordService discordService, ScrimFinderContext dbContext)
        {
            _schedulingService = schedulingService;
            _trackerService = trackerService;
            _steamService = steamService;
            _discordService = discordService;
            _dbContext = dbContext;
        }

        [Route("add")]
        [HttpPost]
        public ScrimEventViewModel AddEvent([FromBody] ScrimEvent scrimEvent)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                var steamId = UserExtensions.GetSteamId(this.User);
                scrimEvent.SteamId = steamId;
                var newEvent = _schedulingService.AddEvent(scrimEvent);
                var newEventViewModel = this.GetEventViewModel(newEvent);
                _schedulingService.NotifyEventAdded(newEventViewModel);
                return newEventViewModel;
            }
            return null;
        }

        [Route("deleteEvent")]
        [HttpPost]
        public IActionResult DeleteEvent([FromBody] int id)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                var steamId = UserExtensions.GetSteamId(this.User);
                _schedulingService.DeleteEvent(steamId, id);
                return Ok();
            }
            return Unauthorized();
        }

        [Route("requestEvent")]
        [HttpPost]
        public ScrimRequestViewModel RequestEvent([FromBody] int id)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                var steamId = UserExtensions.GetSteamId(this.User);
                var request = new ScrimRequest
                {
                    ScrimEventId = id,
                    Date = DateTime.Now,
                    SteamId = steamId
                };
                var newRequest = _schedulingService.AddRequest(request);
                var newRequestViewModel = this.GetRequestViewModel(newRequest);
                _schedulingService.NotifyRequestAdded(id, newRequestViewModel);

                var scrimEvent = _schedulingService.GetScrimEvent(id);
                var user = new UserInfo
                {
                    SteamId = newRequestViewModel.SteamId,
                    DisplayName = newRequestViewModel.DisplayName,
                    Mmr = newRequestViewModel.Mmr
                };

                var userSettings = _dbContext.UserSettings.FirstOrDefault(u => u.SteamId == scrimEvent.OpponentSteamId);
                if (userSettings != null)
                {
                    _discordService.SendMessage(DmType.ScheduleRequest, userSettings.DiscordId, user);
                }

                return newRequestViewModel;
            }
            return null;
        }


        [Route("acceptRequest")]
        [HttpPost]
        public ScrimEventViewModel AcceptRequest([FromBody] int id)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                var refreshedEvent = _schedulingService.AcceptRequest(id);
                var viewModel = this.GetEventViewModel(refreshedEvent);
                _schedulingService.NotifyRequestAccepted(viewModel);

                var currentUser = new UserInfo
                {
                    SteamId = viewModel.SteamId,
                    DisplayName = viewModel.DisplayName,
                    Mmr = viewModel.Mmr
                };
                _discordService.SendMessage(DmType.ScheduleAccept, viewModel.OpponentSteamId, currentUser);

                return viewModel;
            }
            return null;
        }

        [Route("deleteRequest")]
        [HttpPost]
        public IActionResult DeleteRequest([FromBody] int id)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                _schedulingService.DeleteRequest(id);
                return Ok();
            }
            return Unauthorized();
        }

        [Route("sendMessage")]
        [HttpPost]
        public IActionResult SendMessage([FromBody] ChatMessage message)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                var steamId = UserExtensions.GetSteamId(this.User);
                message.SteamId = steamId;
                message.Sent = DateTime.Now;
                _schedulingService.AddMessage(message);
                return Ok();
            }
            return Unauthorized();
        }

        [Route("getScrimEvents")]
        [HttpGet]
        public IEnumerable<ScrimEventViewModel> GetScrimEvents()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                var steamId = UserExtensions.GetSteamId(this.User);
                var scrimEvents = _schedulingService.GetScrimEvents(steamId);
                var viewModels = new List<ScrimEventViewModel>();
                foreach(var scrimEvent in scrimEvents)
                {
                    viewModels.Add(this.GetEventViewModel(scrimEvent));
                }
                return viewModels;
            }
            return null;
        }

        [Route("getLobbyInfo")]
        [HttpPost]
        public LobbyInfo GetLobbyInfo([FromBody] int eventId)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                var steamId = UserExtensions.GetSteamId(this.User);
                return _schedulingService.GetLobbyInfo(eventId, steamId);
            }
            return null;
        }

        private ScrimEventViewModel GetEventViewModel(ScrimEvent scrimEvent)
        {
            var viewModel = new ScrimEventViewModel()
            {
                Id = scrimEvent.Id,
                SteamId = scrimEvent.SteamId,
                OpponentSteamId = scrimEvent.OpponentSteamId,
                TeamName = scrimEvent.TeamName,
                EventDate = scrimEvent.EventDate,
                MatchmakingPreference = scrimEvent.MatchmakingPreference,
                Collegiate = scrimEvent.Collegiate,
                Servers = scrimEvent.Servers,
                ScrimRequests = new List<ScrimRequestViewModel>(),
                ChatLogs = new List<ChatMessageViewModel>()
            };
            viewModel.DisplayName = _steamService.GetSteamDisplayName(scrimEvent.SteamId);
            viewModel.Mmr = _trackerService.GetMmr(scrimEvent.SteamId).Result;
            if (scrimEvent.OpponentSteamId != null)
            {
                viewModel.OpponentDisplayName = _steamService.GetSteamDisplayName(scrimEvent.OpponentSteamId);
                viewModel.OpponentMmr = _trackerService.GetMmr(scrimEvent.OpponentSteamId).Result;
            }

            if (scrimEvent.RequestList != null)
            {
                foreach(var request in scrimEvent.RequestList)
                {
                    var requestViewModel = GetRequestViewModel(request);
                    viewModel.ScrimRequests.Add(requestViewModel);
                }
            }
            if (scrimEvent.ChatLogs != null)
            {
                foreach (var log in scrimEvent.ChatLogs)
                {
                    viewModel.ChatLogs.Add(new ChatMessageViewModel { Id = log.Id, Message = log.Message, Sent = log.Sent, SteamId = log.SteamId });
                }
            }
            return viewModel;
        }

        private ScrimRequestViewModel GetRequestViewModel(ScrimRequest request)
        {
            var requestViewModel = new ScrimRequestViewModel { Id = request.Id, SteamId = request.SteamId, ScrimEventId = request.ScrimEventId };
            requestViewModel.DisplayName = _steamService.GetSteamDisplayName(request.SteamId);
            requestViewModel.Mmr = _trackerService.GetMmr(request.SteamId).Result;
            return requestViewModel;
        }
    }
}