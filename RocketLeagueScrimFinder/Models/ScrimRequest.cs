using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RocketLeagueScrimFinder.Models
{
    public class ScrimRequest
    {
        public int Id { get; set; }
        public string SteamId { get; set; }
        public DateTime Date { get; set; }

        public int ScrimEventId { get; set; }
        public ScrimEvent ScrimEvent { get; set; }
    }
}
