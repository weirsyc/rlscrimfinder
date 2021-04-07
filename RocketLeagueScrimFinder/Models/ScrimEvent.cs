using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RocketLeagueScrimFinder.Models
{
    public class ScrimEvent
    {
        public int Id { get; set; }
        public string SteamId { get; set; }
        public string OpponentSteamId { get; set; }
        public string TeamName { get; set; }
        public DateTime EventDate { get; set; }
        public int MatchmakingPreference { get; set; }
        public bool Collegiate { get; set; }
        public string Servers { get; set; }
        public IEnumerable<ScrimRequest> RequestList { get; set; }
        public IEnumerable<ChatMessage> ChatLogs { get; set; }
    }
}
