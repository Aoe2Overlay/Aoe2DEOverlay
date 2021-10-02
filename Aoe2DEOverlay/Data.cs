using System;
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
    public class Player
    {
        public int Id;
        public int Slot;
        public int Color;
        public string Name = "";
        public string Country = "";
        public string Civ = "";
        public Raiting RM1v1 = new Raiting();
        public Raiting RMTeam = new Raiting();
        public Raiting EW1v1 = new Raiting();
        public Raiting EWTeam = new Raiting();
    }

    public class Raiting
    {
        public int Elo = 1000;
        public int Rank = 0;
        public int Streak = 0;
        public int Games = 0;
        public int Wins = 0;
        public int Losses = 0;
        public int WinRate => Games == 0 ? 0 : (int) Math.Round (100f / Games * Wins);
        public int LastMatchTime = 0;
    }
}