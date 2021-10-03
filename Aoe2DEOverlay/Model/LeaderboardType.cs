namespace Aoe2DEOverlay
{
    public enum LeaderboardType
    {
        Unranked = 0,
        Deathmatch1v1 = 1,
        DeathmatchTeam  = 2,
        RandomMap1v1 = 3,
        RandomMapTeam = 4,
        EmpireWars1v1  = 13,
        EmpireWarsTeam  = 14
    }
    
    public static class LeaderboardTypeExtensions
    {
        public static string ToModeName(this LeaderboardType type)
        {
            switch(type)
            {
                case LeaderboardType.Unranked:
                    return "Unranked";
                case LeaderboardType.Deathmatch1v1:
                case LeaderboardType.DeathmatchTeam:
                    return "Deathmatch";
                case LeaderboardType.RandomMap1v1:
                case LeaderboardType.RandomMapTeam:
                    return "Random Map";
                case LeaderboardType.EmpireWars1v1:
                case LeaderboardType.EmpireWarsTeam:
                    return "Empire Wars";
            }
            return "Unknown";
        }
        
        public static string ToModeShort(this LeaderboardType type)
        {
            switch(type)
            {
                case LeaderboardType.Unranked:
                    return "UR";
                case LeaderboardType.Deathmatch1v1:
                case LeaderboardType.DeathmatchTeam:
                    return "DM";
                case LeaderboardType.RandomMap1v1:
                case LeaderboardType.RandomMapTeam:
                    return "RM";
                case LeaderboardType.EmpireWars1v1:
                case LeaderboardType.EmpireWarsTeam:
                    return "EW";
            }
            return "";
        }
    }
}