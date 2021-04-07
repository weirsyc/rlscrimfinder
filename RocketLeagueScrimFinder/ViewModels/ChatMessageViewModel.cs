using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RocketLeagueScrimFinder.ViewModels
{
    public class ChatMessageViewModel
    {
        public int Id { get; set; }
        public DateTime Sent { get; set; }
        public string SteamId { get; set; }
        public int ScrimEventId { get; set; }
        public string Message { get; set; }
    }
}
