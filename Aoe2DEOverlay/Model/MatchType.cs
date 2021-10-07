namespace Aoe2DEOverlay
{
    public enum MatchType
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
        public static string ToModeName(this MatchType type)
        {
            switch(type)
            {
                case MatchType.Unranked:
                    return "Unranked";
                case MatchType.Deathmatch1v1:
                case MatchType.DeathmatchTeam:
                    return "Deathmatch";
                case MatchType.RandomMap1v1:
                case MatchType.RandomMapTeam:
                    return "Random Map";
                case MatchType.EmpireWars1v1:
                case MatchType.EmpireWarsTeam:
                    return "Empire Wars";
            }
            return "Unknown";
        }
        
        public static string ToModeShort(this MatchType type)
        {
            switch(type)
            {
                case MatchType.Unranked:
                    return "UR";
                case MatchType.Deathmatch1v1:
                case MatchType.DeathmatchTeam:
                    return "DM";
                case MatchType.RandomMap1v1:
                case MatchType.RandomMapTeam:
                    return "RM";
                case MatchType.EmpireWars1v1:
                case MatchType.EmpireWarsTeam:
                    return "EW";
            }
            return "";
        }
    }
}