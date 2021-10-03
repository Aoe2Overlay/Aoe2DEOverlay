using System.Collections.Generic;

namespace Aoe2DEOverlay
{
    public class Data
    {
        public int LeaderboardId;
        public string ServerKey;
        public string ServerName;
        public string MatchModeName; // Random Map, Empire Wars, Unranked
        public string MatchModeShort; // RM, EW, UR
        public string MapName;
        public List<Player> players = new (8);
    }
}