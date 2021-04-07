using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RocketLeagueScrimFinder.Models
{
    public class LobbyInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public int ScrimEventId { get; set; }
        public ScrimEvent ScrimEvent { get; set; }
    }
}
