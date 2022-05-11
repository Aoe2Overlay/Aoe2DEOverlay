using System.Collections.Generic;

namespace Aoe2DEOverlay
{
    public class Match
    {
        public ulong? SteamId = null;
        public uint? ProfileId = null;
        
        public uint Started;
        public bool HasAI = false;
        public string Difficulty;
        public bool IsMultiplayer;
        public bool IsRanked;
        public string ServerKey;
        public string ServerName;
        public string GameTypeName; // Random Map, Empire Wars, Unranked
        public string GameTypeShort; // RM, EW, UR
        public int MapType;
        public string MapName;
        public List<Player> Players = new (8);
        
        
        public bool IsRecordRead = true;
    }
}