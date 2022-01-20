using System;

namespace Aoe2DEOverlay
{
    public class Raiting
    {
        public int Elo = 0;
        public int Rank = 0;
        public int Streak = 0;
        public int Games = 0;
        public int Wins = 0;
        public int Losses = 0;
        public int WinRate => Games == 0 ? 0 : (int) Math.Round (100f / Games * Wins);
        public int LastMatchTime = 0;
        public bool IsActive = false;
    }
}