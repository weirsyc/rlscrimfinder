using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RocketLeagueScrimFinder.Models;
using RocketLeagueScrimFinder.SignalR;
using RocketLeagueScrimFinder.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RocketLeagueScrimFinder.Services
{
    public class SchedulingService
    {
        private ScrimFinderContext _dbContext;

        private IHubContext<AppHub> _hubContext;

        public SchedulingService(IHubContext<AppHub> hubContext, ScrimFinderContext dbContext)
        {
            _dbContext = dbContext;      
            _hubContext = hubContext;
        }

        internal ScrimEvent AddEvent(ScrimEvent scrimEvent)
        {
            var newEvent = _dbContext.Add(scrimEvent);
            _dbContext.SaveChanges();
            return newEvent.Entity;
        }

        internal void DeleteEvent(string steamId, int eventId)
        {
            var scrimEvent = _dbContext.ScrimEvents.FirstOrDefault(e => e.Id == eventId && e.SteamId == steamId);
            if (scrimEvent != null)
            {
                _dbContext.Remove(scrimEvent);
                _dbContext.SaveChanges();
                _hubContext.Clients.All.SendAsync("EventDeleted", eventId);
            }
        }

        private ScrimEvent GetScrimEvent(int id)
        {
            return _dbContext.ScrimEvents.Include(s => s.RequestList).Include(s => s.ChatLogs).FirstOrDefault(e => e.Id == id);
        }

        internal IEnumerable<ScrimEvent> GetScrimEvents(string steamId)
        {
            return _dbContext.ScrimEvents.Include(s => s.RequestList).Include(s => s.ChatLogs).Where(e => 
                e.EventDate >= DateTime.UtcNow.Date.AddDays(-1) && (e.SteamId == steamId || e.OpponentSteamId == steamId || e.OpponentSteamId == null));
        }

        internal ScrimRequest AddRequest(ScrimRequest request)
        {
            var newRequest = _dbContext.Add(request);
            _dbContext.SaveChanges();
            return newRequest.Entity;
        }

        internal ScrimEvent AcceptRequest(int id)
        {
            var scrimRequest = _dbContext.ScrimRequests.Include(s => s.ScrimEvent).FirstOrDefault(r => r.Id == id);
            scrimRequest.ScrimEvent.OpponentSteamId = scrimRequest.SteamId;
            _dbContext.SaveChanges();
            return GetScrimEvent(scrimRequest.ScrimEventId);
        }

        internal void NotifyRequestAdded(int id, ScrimRequestViewModel requestViewModel)
        {
            var recipient = _dbContext.ScrimEvents.FirstOrDefault(e => e.Id == id);
            foreach (var client in UserConnections.GetConnections(recipient.SteamId))
            {
                _hubContext.Clients.Client(client).SendAsync("RequestAdded", requestViewModel);
            }
        }

        internal void NotifyRequestAccepted(ScrimEventViewModel viewModel)
        {
            _hubContext.Clients.All.SendAsync("RequestAccepted", viewModel);
        }
        internal void NotifyEventAdded(ScrimEventViewModel viewModel)
        {
            _hubContext.Clients.All.SendAsync("EventAdded", viewModel);
        }

        internal void DeleteRequest(int id)
        {
            var req = _dbContext.ScrimRequests.Include(r => r.ScrimEvent).FirstOrDefault(r => r.Id == id);
            if (req != null)
            {
                _dbContext.Remove(req);
                _dbContext.SaveChanges();
                foreach (var client in UserConnections.GetConnections(req.ScrimEvent.SteamId))
                {
                    _hubContext.Clients.Client(client).SendAsync("RequestDeleted", req.Id);
                }
                foreach (var client in UserConnections.GetConnections(req.SteamId))
                {
                    _hubContext.Clients.Client(client).SendAsync("RequestDeleted", req.Id);
                }
            }
        }

        internal void AddMessage(ChatMessage message)
        {
            _dbContext.ChatMessages.Add(message);
            _dbContext.SaveChanges();
            var scrimEvent = _dbContext.ScrimEvents.FirstOrDefault(e => e.Id == message.ScrimEventId);
            var messageViewModel = new ChatMessageViewModel
            {
                Id = message.Id,
                Message = message.Message,
                Sent = message.Sent,
                SteamId = message.SteamId,
                ScrimEventId = message.ScrimEventId
            };
            foreach (var client in UserConnections.GetConnections(scrimEvent.SteamId))
            {
                _hubContext.Clients.Client(client).SendAsync("MessageReceived", messageViewModel);
            }
            foreach (var client in UserConnections.GetConnections(scrimEvent.OpponentSteamId))
            {
                _hubContext.Clients.Client(client).SendAsync("MessageReceived", messageViewModel);
            }
        }

        internal LobbyInfo GetLobbyInfo(int eventId, string steamId)
        {
            var lobbyInfo = _dbContext.LobbyInfos.FirstOrDefault(i => i.ScrimEventId == eventId);
            if (lobbyInfo != null)
            {
                return lobbyInfo;
            }
            var existingInfo = _dbContext.ScrimEvents.FirstOrDefault(i => i.Id == eventId && (i.SteamId == steamId || i.OpponentSteamId == steamId));
            if (existingInfo != null)
            {
                var li = new LobbyInfo { Name = $"RLSF{eventId % 1000}", Password = GeneratePassword(), ScrimEventId = eventId };
                var newLobbyInfo = _dbContext.LobbyInfos.Add(li);
                _dbContext.SaveChanges();
                return newLobbyInfo.Entity;
            }
            return null;
        }

        private string GeneratePassword()
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, 4).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
