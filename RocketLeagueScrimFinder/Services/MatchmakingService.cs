using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using RocketLeagueScrimFinder.Data;
using RocketLeagueScrimFinder.Models;
using RocketLeagueScrimFinder.SignalR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RocketLeagueScrimFinder.Services
{
    public class MatchmakingService
    {
        private int lobbyCounter = 1;
        private IList<UserInfo> _playerQueueList;
        private IList<UserInfo> _playersInMatch;
        private IList<MatchmakingData> _lobbyList;

        //keep track of players who already declined playing against each other
        private IMemoryCache _declineListCache;
        private readonly object _lockObj = new object();
        private IHubContext<AppHub> _hubContext;
        private DiscordService _discordService;
        private readonly IServiceScopeFactory _scopeFactory;

        public MatchmakingService(IHubContext<AppHub> hubContext, IMemoryCache declineListCache, DiscordService discordService, IServiceScopeFactory scopeFactory)
        {
            _playerQueueList = new List<UserInfo>();
            _playersInMatch = new List<UserInfo>();
            _lobbyList = new List<MatchmakingData>();
            _declineListCache = declineListCache;             
            _hubContext = hubContext;
            _scopeFactory = scopeFactory;
            _discordService = discordService;
        }

        internal void UpdatePreference(string steamId, int matchMakingPreference)
        {
            lock (_lockObj)
            {
                var queuedPlayer = _playerQueueList.FirstOrDefault(p => p.SteamId == steamId);
                if (queuedPlayer != null)
                {
                    queuedPlayer.MatchmakingPreference = matchMakingPreference;
                    var foundOpponent = SearchForOpponent(queuedPlayer);
                    if (!foundOpponent)
                    {
                        _hubContext.Clients.All.SendAsync("PlayerListUpdate", queuedPlayer);
                    }
                }
            }
        }

        internal void SendMessage(string steamId, string message)
        {
            lock (_lockObj)
            {
                var lobbyInfo = _lobbyList.FirstOrDefault(l => (l.Player1SteamId == steamId && !l.Player1Left) || (l.Player2SteamId == steamId && !l.Player2Left));
                var newMessage = new Models.ChatMessage { SteamId = steamId, Message = message, Sent = DateTime.Now };
                lobbyInfo.ChatLog.Add(newMessage);
                foreach (var client in UserConnections.GetConnections(lobbyInfo.Player1SteamId))
                {
                    _hubContext.Clients.Client(client).SendAsync("MessageReceived", newMessage);
                }
                foreach (var client in UserConnections.GetConnections(lobbyInfo.Player2SteamId))
                {
                    _hubContext.Clients.Client(client).SendAsync("MessageReceived", newMessage);
                }
            }
        }

        internal void UpdateServerSelection(string steamId, IEnumerable<int> selectedServers)
        {
            lock (_lockObj)
            {
                var queuedPlayer = _playerQueueList.FirstOrDefault(p => p.SteamId == steamId);
                if (queuedPlayer != null)
                {
                    queuedPlayer.Servers = selectedServers;
                    var foundOpponent = SearchForOpponent(queuedPlayer);
                    if (!foundOpponent)
                    {
                        _hubContext.Clients.All.SendAsync("PlayerListUpdate", queuedPlayer);
                    }
                }
            }
        }
        internal void UpdateCollegiateSelected(string steamId, bool collegiateSelected)
        {
            lock (_lockObj)
            {
                var queuedPlayer = _playerQueueList.FirstOrDefault(p => p.SteamId == steamId);
                if (queuedPlayer != null)
                {
                    queuedPlayer.Collegiate = collegiateSelected;
                    var foundOpponent = SearchForOpponent(queuedPlayer);
                    if (!foundOpponent)
                    {
                        _hubContext.Clients.All.SendAsync("PlayerListUpdate", queuedPlayer);
                    }
                }
            }
        }

        internal void StartSearch(UserInfo userInfo)
        {
            lock (_lockObj)
            {
                var opponentFound = SearchForOpponent(userInfo);
                if (!opponentFound && _playerQueueList.FirstOrDefault(p => p.SteamId == userInfo.SteamId) == null)
                {
                    _playerQueueList.Add(userInfo);
                    _hubContext.Clients.All.SendAsync("PlayerListUpdate", userInfo);
                }
            }
        }

        internal MatchmakingData GetExistingLobbyInfo(string steamId)
        {
            lock (_lockObj)
            {
                return _lobbyList.FirstOrDefault(l => (l.Player1SteamId == steamId && !l.Player1Left) || (l.Player2SteamId == steamId && !l.Player2Left));
            }
        }

        internal bool GetIsQueued(string steamId)
        {
            lock (_lockObj)
            {
                return _playerQueueList.FirstOrDefault(s => s.SteamId == steamId) != null;
            }
        }

        internal void LeaveLobby(string steamId)
        {
            lock (_lockObj)
            {
                var playerInMatch = _playersInMatch.FirstOrDefault(p => p.SteamId == steamId);
                if (playerInMatch != null) {
                    _playersInMatch.Remove(playerInMatch);
                }

                var lobbyInfo = _lobbyList.FirstOrDefault(l => (l.Player1SteamId == steamId && !l.Player1Left) || (l.Player2SteamId == steamId && !l.Player2Left));
                var pairIdentifier = string.Join('_', new List<string> { lobbyInfo.Player1SteamId, lobbyInfo.Player2SteamId }.OrderBy(s => s));
                if (lobbyInfo == null)
                {
                    return;
                }

                if (lobbyInfo.Player1SteamId == steamId)
                {
                    if (lobbyInfo.Player2Left)
                    {
                        _lobbyList.Remove(lobbyInfo);
                        _declineListCache.Set(pairIdentifier, true, DateTime.Now.AddMinutes(30));
                    }
                    else
                    {
                        lobbyInfo.Player1Left = true;
                        foreach (var client in UserConnections.GetConnections(lobbyInfo.Player2SteamId))
                        {
                            _hubContext.Clients.Client(client).SendAsync("OpponentLeftLobby", lobbyInfo.Player1SteamId);
                        }
                    }
                }
                else
                {
                    if (lobbyInfo.Player1Left)
                    {
                        _lobbyList.Remove(lobbyInfo);
                        _declineListCache.Set(pairIdentifier, true, DateTime.Now.AddMinutes(30));
                    }
                    else
                    {
                        lobbyInfo.Player2Left = true;
                        foreach (var client in UserConnections.GetConnections(lobbyInfo.Player1SteamId))
                        {
                            _hubContext.Clients.Client(client).SendAsync("OpponentLeftLobby", lobbyInfo.Player2SteamId);
                        }
                    }
                }
            }
        }

        internal IEnumerable<UserInfo> RetrieveQueuedPlayers()
        {
            lock (_lockObj)
            {
                return _playerQueueList;
            }
        }

        internal void AcceptMatch(string steamId)
        {
            lock (_lockObj)
            {
                var lobbyInfo = _lobbyList.FirstOrDefault(l => (l.Player1SteamId == steamId && !l.Player1Left) || (l.Player2SteamId == steamId && !l.Player2Left));
                if (lobbyInfo == null)
                {
                    return;
                }

                if (lobbyInfo.Player1SteamId == steamId)
                {
                    lobbyInfo.Player1MatchAccepted = true;
                }
                if (lobbyInfo.Player2SteamId == steamId)
                {
                    lobbyInfo.Player2MatchAccepted = true;
                }

                if (lobbyInfo.Player1MatchAccepted && lobbyInfo.Player2MatchAccepted)
                {
                    foreach (var client in UserConnections.GetConnections(lobbyInfo.Player1SteamId))
                    {
                        _hubContext.Clients.Client(client).SendAsync("EnterLobby", true);
                    }
                    foreach (var client in UserConnections.GetConnections(lobbyInfo.Player2SteamId))
                    {
                        _hubContext.Clients.Client(client).SendAsync("EnterLobby", true);
                    }
                    this.SendMatchAcceptedNotifications(DmType.MatchAccepted, lobbyInfo);
                }
            }
        }

        private void SendMatchAcceptedNotifications(DmType dmType, MatchmakingData lobbyInfo)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ScrimFinderContext>();

                var user1 = new UserInfo
                {
                    SteamId = lobbyInfo.Player1SteamId,
                    DisplayName = lobbyInfo.Player1Name,
                    Mmr = lobbyInfo.Player1Mmr
                };
                var user2 = new UserInfo
                {
                    SteamId = lobbyInfo.Player2SteamId,
                    DisplayName = lobbyInfo.Player2Name,
                    Mmr = lobbyInfo.Player2Mmr
                };
                var userSettings1 = dbContext.UserSettings.FirstOrDefault(u => u.SteamId == user1.SteamId);
                var userSettings2 = dbContext.UserSettings.FirstOrDefault(u => u.SteamId == user2.SteamId);
                if (userSettings1 != null)
                {
                    _discordService.SendMessage(dmType, userSettings1.DiscordId, user2);
                }
                if (userSettings2 != null)
                {
                    _discordService.SendMessage(dmType, userSettings2.DiscordId, user1);
                }
            }
        }

        internal void DeclineMatch(string steamId)
        {
            lock (_lockObj)
            {
                var lobbyInfo = _lobbyList.FirstOrDefault(l => (l.Player1SteamId == steamId && !l.Player1Left) || (l.Player2SteamId == steamId && !l.Player2Left));
                if (lobbyInfo == null)
                {
                    return;
                }

                var pairIdentifier = string.Join('_', new List<string> { lobbyInfo.Player1SteamId, lobbyInfo.Player2SteamId }.OrderBy(s => s));
                _declineListCache.Set(pairIdentifier, true, DateTime.Now.AddMinutes(30));

                _lobbyList.Remove(lobbyInfo);

                foreach (var client in UserConnections.GetConnections(lobbyInfo.Player1SteamId))
                {
                    _hubContext.Clients.Client(client).SendAsync("DeclinedLobby", true);
                }
                foreach (var client in UserConnections.GetConnections(lobbyInfo.Player2SteamId))
                {
                    _hubContext.Clients.Client(client).SendAsync("DeclinedLobby", true);
                }

                //Place both users back in the queue
                var player1InMatch = _playersInMatch.FirstOrDefault(p => p.SteamId == lobbyInfo.Player1SteamId);
                if (player1InMatch != null)
                {
                    _playersInMatch.Remove(player1InMatch);
                    StartSearch(player1InMatch);
                }
                var player2InMatch = _playersInMatch.FirstOrDefault(p => p.SteamId == lobbyInfo.Player2SteamId);
                if (player2InMatch != null)
                {
                    _playersInMatch.Remove(player2InMatch);
                    StartSearch(player2InMatch);
                }
            }
        }

        private bool SearchForOpponent(UserInfo searchingPlayer)
        {
            foreach (var queuedPlayer in _playerQueueList.Where(p => p.SteamId != searchingPlayer.SteamId))
            {
                if (SearchCriteriaMatches(searchingPlayer, queuedPlayer))
                {
                    var matchMakingData = new MatchmakingData
                    {
                        Player1SteamId = searchingPlayer.SteamId,
                        Player1Mmr = searchingPlayer.Mmr,
                        Player1Name = searchingPlayer.DisplayName,
                        Player2SteamId = queuedPlayer.SteamId,
                        Player2Mmr = queuedPlayer.Mmr,
                        Player2Name = queuedPlayer.DisplayName,
                        LobbyName = $"RLSF{lobbyCounter}",
                        LobbyPassword = GeneratePassword(),
                        ServerChoices = queuedPlayer.Servers.Intersect(searchingPlayer.Servers).ToList(),
                        ChatLog = new List<Models.ChatMessage>(),
                        Player1MatchAccepted = false,
                        Player2MatchAccepted = false,
                    };
                    foreach (var client in UserConnections.GetConnections(searchingPlayer.SteamId))
                    {
                        _hubContext.Clients.Client(client).SendAsync("MatchFound", matchMakingData);
                    }
                    foreach (var client in UserConnections.GetConnections(queuedPlayer.SteamId))
                    {
                        _hubContext.Clients.Client(client).SendAsync("MatchFound", matchMakingData);
                    }
                    this.SendMatchAcceptedNotifications(DmType.MatchFound, matchMakingData);
                    lobbyCounter++;
                    if (lobbyCounter == 1000)
                    {
                        lobbyCounter = 1;
                    }

                    //Remove players from queue
                    foreach (var player in _playerQueueList.Where(p => p.SteamId == searchingPlayer.SteamId || p.SteamId == queuedPlayer.SteamId).ToList())
                    {
                        _playerQueueList.Remove(player);
                        _hubContext.Clients.All.SendAsync("PlayerListRemoval", player);
                    }

                    _playersInMatch.Add(searchingPlayer);
                    _playersInMatch.Add(queuedPlayer);
                    _lobbyList.Add(matchMakingData);

                    return true;
                }
            }
            return false;
        }

        private string GeneratePassword()
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, 4).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private bool SearchCriteriaMatches(UserInfo searchingPlayer, UserInfo queuedPlayer)
        {
            var mmrMatches = (searchingPlayer.Mmr >= queuedPlayer.GetLowMmrTarget() && searchingPlayer.Mmr <= queuedPlayer.GetHighMmrTarget()) &&
                (queuedPlayer.Mmr >= searchingPlayer.GetLowMmrTarget() && queuedPlayer.Mmr <= searchingPlayer.GetHighMmrTarget());
            var serversMatch = searchingPlayer.Servers.Intersect(queuedPlayer.Servers).Any();
            var collegiateMatch = searchingPlayer.Collegiate == queuedPlayer.Collegiate;
            
            var pairIdentifier = string.Join('_', new List<string> { searchingPlayer.SteamId, queuedPlayer.SteamId }.OrderBy(s => s));
            var inDeclineList = _declineListCache.TryGetValue(pairIdentifier, out var x);
            return mmrMatches && serversMatch && collegiateMatch && !inDeclineList;
        }

        internal void CancelSearch(string steamId)
        {
            lock (_lockObj)
            {
                var queuedPlayer = _playerQueueList.FirstOrDefault(p => p.SteamId == steamId);
                if (queuedPlayer != null)
                {
                    _playerQueueList.Remove(queuedPlayer);
                    _hubContext.Clients.All.SendAsync("PlayerListRemoval", queuedPlayer);
                }
            }
        }

        internal IList<UserInfo> GetQueueData()
        {
            lock (_lockObj)
            {
                return this._playerQueueList;
            }
        }
        internal IList<UserInfo> GetPlayersInMatch()
        {
            lock (_lockObj)
            {
                return this._playersInMatch;
            }
        }
        internal IList<MatchmakingData> GetLobbies()
        {
            lock (_lockObj)
            {
                return this._lobbyList;
            }
        }
        internal IList<string> GetDeclineList()
        {
            lock (_lockObj)
            {
                var field = typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
                var collection = field.GetValue(_declineListCache) as ICollection;
                var items = new List<string>();
                if (collection != null)
                    foreach (var item in collection)
                    {
                        var methodInfo = item.GetType().GetProperty("Key");
                        var val = methodInfo.GetValue(item);
                        if (val.ToString().Contains('_'))
                        {
                            items.Add(val.ToString());
                        }
                    }
                return items;
            }
        }
    }
}
