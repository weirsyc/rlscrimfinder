using RocketLeagueScrimFinder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RocketLeagueScrimFinder.ViewModels
{
    public class ScrimEventViewModel
    {
        public int Id { get; set; }
        public string SteamId { get; set; }
        public string OpponentSteamId { get; set; }
        public string TeamName { get; set; }
        public DateTime EventDate { get; set; }
        public int MatchmakingPreference { get; set; }
        public bool Collegiate { get; set; }
        public string Servers { get; set; }
        public IList<ScrimRequestViewModel> ScrimRequests { get; set; }
        public IList<ChatMessageViewModel> ChatLogs { get; set; }
        public string DisplayName { get; set; }
        public string OpponentDisplayName { get; set; }
        public int Mmr { get; set; }
        public int OpponentMmr { get; set; }
    }
}
