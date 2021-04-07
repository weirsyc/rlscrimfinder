using RocketLeagueScrimFinder.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RocketLeagueScrimFinder.Areas.Management.Models
{
    public class ManagementViewModel
    {
        public IList<UserInfo> PlayerQueueList { get; set; }
        public IList<UserInfo> PlayersInMatch { get; set; }
        public IList<MatchmakingData> LobbyList { get; set; }
        public IList<string> DeclineList { get; set; }
    }
}
