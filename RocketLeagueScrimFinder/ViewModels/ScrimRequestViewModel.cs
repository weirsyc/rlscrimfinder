using RocketLeagueScrimFinder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RocketLeagueScrimFinder.ViewModels
{
    public class ScrimRequestViewModel
    {
        public int Id { get; set; }
        public string SteamId { get; set; }
        public string DisplayName { get; set; }
        public int Mmr { get; set; }
        public int ScrimEventId { get; set; }
    }
}
