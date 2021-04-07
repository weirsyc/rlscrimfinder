using System.Collections.Generic;

namespace RocketLeagueScrimFinder.Data
{
    public class UserInfo
    {
        public string SteamId { get; set; }
        public string DisplayName { get; set; }
        public int Mmr { get; set; }
        //0 - close, 1 - medium, 2 - far, 3 - none
        public int MatchmakingPreference { get; set; }
        public IEnumerable<int> Servers { get; set; }
        public bool Collegiate { get; set; }

        private const int CLOSE = 50;
        private const int MEDIUM = 100;
        private const int FAR = 200;

        public int GetLowMmrTarget()
        {
            switch (MatchmakingPreference)
            {
                case 0:
                    return Mmr - CLOSE;
                case 1:
                    return Mmr - MEDIUM;
                case 2:
                    return Mmr - FAR;
                default:
                    return int.MinValue;
            }
        }
        public int GetHighMmrTarget()
        {
            switch (MatchmakingPreference)
            {
                case 0:
                    return Mmr + CLOSE;
                case 1: 
                    return Mmr + MEDIUM;
                case 2:
                    return Mmr + FAR;
                default: 
                    return int.MaxValue;
            }
        }
    }
}
