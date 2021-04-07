using System.Collections.Generic;

namespace RocketLeagueScrimFinder.Data
{
    public class MatchmakingData
    {
        public string Player1SteamId { get; set; }
        public string Player1Name { get; set; }
        public int Player1Mmr { get; set; }
        public string Player2SteamId { get; set; }
        public string Player2Name { get; set; }
        public int Player2Mmr { get; set; }
        public bool Player1Left { get; set; }
        public bool Player2Left { get; set; }
        public List<int> ServerChoices { get; set; }
        public string LobbyName { get; set; }
        public string LobbyPassword { get; set; }
        public List<Models.ChatMessage> ChatLog { get; set; }
        public bool Player1MatchAccepted { get; set; }
        public bool Player2MatchAccepted { get; set; }
    }
}
