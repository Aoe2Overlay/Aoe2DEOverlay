using System;
using System.IO;
using System.Linq;

namespace ReadAoe2Recrod
{
    public partial class Aoe2Record
    {
        public string Difficulty;
        public string MapName;
        public bool IsMultiplayer;
        public bool IsRanked;
        
        public uint NumberOfPlayers;

        private void ReadMatchPart1(BinaryReader reader)
        {
            var datasetRef = reader.ReadUInt32();
            var difficultyId = reader.ReadUInt32();
            Difficulty = ParseDefficulty(difficultyId);
            var selectedMapId = reader.ReadUInt32();
            var resolvedMapId = reader.ReadUInt32();
            MapName = ParseMapName(resolvedMapId);
            var revealMap = BoolUInt32(reader);
            var victoryTypeId = reader.ReadUInt32();
            var victoryType = ParseVictoryType(victoryTypeId);
            var startingResourcesId = reader.ReadInt32();
            var startingResources = ParseStartingResources(startingResourcesId);
            var startingAgeId = (int)reader.ReadUInt32() - 2;
            var startingAge = ParseStartingAge(startingAgeId);
            var endingAgeId = (int)reader.ReadUInt32() - 2;
            var endingAge = ParseStartingAge(endingAgeId);
            var gameTypeId = reader.ReadUInt32();
            Padding(reader, 4*2); // separator * 2
            var speed = Math.Round(reader.ReadSingle(), 1);
            var treatyLength = reader.ReadUInt32();
            var populationLimit = reader.ReadUInt32();
            NumberOfPlayers = reader.ReadUInt32();
            var unusedPlayerColor = reader.ReadUInt32();
            var victoryAmount = reader.ReadUInt32();
            Padding(reader, 4); // separator
            var tradeEnabled = reader.ReadBoolean();
            var teamBonusDisabled = reader.ReadBoolean();
            var randomPositions = reader.ReadBoolean();
            var allTechs = reader.ReadBoolean();
            var numStartingUnits = reader.ReadByte();
            var lockTeams = reader.ReadBoolean();
            var lockSpeed = reader.ReadBoolean();
            IsMultiplayer = reader.ReadBoolean();
            var cheats = reader.ReadBoolean();
            var recordGame = reader.ReadBoolean();
            var animalsEnabled = reader.ReadBoolean();
            var predatorsEnabled = reader.ReadBoolean();
            var turboEnabled = reader.ReadBoolean();
            var sharedExploration = reader.ReadBoolean();
            var teamPositions = reader.ReadBoolean();
        }

        private void ReadMatchPart2(BinaryReader reader)
        {
            var fogOfWar = reader.ReadBoolean();
            var cheatNotifications = reader.ReadBoolean();
            var coloredChat = reader.ReadBoolean();
            Padding(reader, 9);
            Padding(reader, 4); // separator
            IsRanked = reader.ReadBoolean();
            Padding(reader, 11);
            Padding(reader, 5);
            var strings = new string[23];
            for (int i = 0; i < 23; i++)
            {
                strings[i] = DEString(reader);
                while (new uint[] {3, 21, 23, 42, 44, 45, 46}.Contains(reader.ReadUInt32())){}
            }
            Padding(reader, 59*4); // skip strategic_numbers ArrayInt32(reader, 59)
            var num_ai_files = (int)reader.ReadUInt64();
            for (int i = 0; i < num_ai_files; i++)
            {
                Padding(reader, 4);
                DEString(reader);
                Padding(reader, 4);
            }
            Padding(reader, 24);
            var lobby_name = DEString(reader);
            var modded_dataset = DEString(reader);
            Padding(reader, 42);
            DEString(reader);
            Padding(reader, 8);
        }

        public string ParseDefficulty(uint id)
        {
            if (id == 0) return "Hardest"; // 1000 ELO
            if (id == 1) return "Hard"; // 900 ELO
            if (id == 2) return "Moderate"; // 700 ELO
            if (id == 3) return "Standard"; // 400 ELO
            if (id == 4) return "Easiest"; // 100 ELO
            if (id == 5) return "Extreme"; // 1100 ELO
            return "Unknown";
        }

        public string ParseVictoryType(uint id)
        {
            if (id == 0) return "Standard";
            if (id == 1) return "Conquest";
            if (id == 2) return "Exploration";
            if (id == 3) return "Ruins";
            if (id == 4) return "Artifacts";
            if (id == 5) return "Discoveries";
            if (id == 6) return "Gold";
            if (id == 7) return "Time Limit";
            if (id == 8) return "Score";
            if (id == 9) return "Standard";
            if (id == 10) return "Regicide";
            if (id == 11) return "Last Man";
            return "Unknown";
        }

        public string ParseStartingResources(int id)
        {
            if (id == -1) return "None";
            if (id == 0) return "Standard";
            if (id == 1) return "Low";
            if (id == 2) return "Medium";
            if (id == 3) return "High";
            if (id == 4) return "Unknown1";
            if (id == 5) return "Unknown2";
            if (id == 6) return "Unknown3";
            return "Unknown";
        }

        public string ParseStartingAge(int id)
        {
            if (id == -2) return "What";
            if (id == -1) return "Unset";
            if (id == 0) return "Dark";
            if (id == 1) return "Feudal";
            if (id == 2) return "Castle";
            if (id == 3) return "Imperial";
            if (id == 4) return "Post-Imperial";
            if (id == 6) return "DM Post-Imperial";
            return "Unknown";
        }

        private string ParseMapName(uint id)
        {
            if(id == 9) return "Arabia";
            if(id == 10) return "Archipelago";
            if(id == 11) return "Baltic";
            if(id == 12) return "Black Forest";
            if(id == 13) return "Coastal";
            if(id == 14) return "Continental";
            if(id == 15) return "Crater Lake";
            if(id == 16) return "Fortress";
            if(id == 17) return "Gold Rush";
            if(id == 18) return "Highland";
            if(id == 19) return "Islands";
            if(id == 20) return "Mediterranean";
            if(id == 21) return "Migration";
            if(id == 22) return "Rivers";
            if(id == 23) return "Team Islands";
            if(id == 24) return "Full Random";
            if(id == 25) return "Scandinavia";
            if(id == 26) return "Mongolia";
            if(id == 27) return "Yucatan";
            if(id == 28) return "Salt Marsh";
            if(id == 29) return "Arena";
            if(id == 30) return "King of the Hill";
            if(id == 31) return "Oasis";
            if(id == 32) return "Ghost Lake";
            if(id == 33) return "Nomad";
            if(id == 49) return "Iberia";
            if(id == 50) return "Britain";
            if(id == 51) return "Mideast";
            if(id == 52) return "Texas";
            if(id == 53) return "Italy";
            if(id == 54) return "Central America";
            if(id == 55) return "France";
            if(id == 56) return "Norse Lands";
            if(id == 57) return "Sea of Japan (East Sea)";
            if(id == 58) return "Byzantium";
            if(id == 59) return "Custom";
            if(id == 60) return "Random Land Map";
            if(id == 62) return "Random Real World Map";
            if(id == 63) return "Blind Random";
            if(id == 65) return "Random Special Map";
            if(id == 66) return "Random Special Map";
            if(id == 67) return "Acropolis";
            if(id == 68) return "Budapest";
            if(id == 69) return "Cenotes";
            if(id == 70) return "City of Lakes";
            if(id == 71) return "Golden Pit";
            if(id == 72) return "Hideout";
            if(id == 73) return "Hill Fort";
            if(id == 74) return "Lombardia";
            if(id == 75) return "Steppe";
            if(id == 76) return "Valley";
            if(id == 77) return "MegaRandom";
            if(id == 78) return "Hamburger";
            if(id == 79) return "CtR Random";
            if(id == 80) return "CtR Monsoon";
            if(id == 81) return "CtR Pyramid Descent";
            if(id == 82) return "CtR Spiral";
            if(id == 83) return "Kilimanjaro";
            if(id == 84) return "Mountain Pass";
            if(id == 85) return "Nile Delta";
            if(id == 86) return "Serengeti";
            if(id == 87) return "Socotra";
            if(id == 88) return "Amazon";
            if(id == 89) return "China";
            if(id == 90) return "Horn of Africa";
            if(id == 91) return "India";
            if(id == 92) return "Madagascar";
            if(id == 93) return "West Africa";
            if(id == 94) return "Bohemia";
            if(id == 95) return "Earth";
            if(id == 96) return "Canyons";
            if(id == 97) return "Enemy Archipelago";
            if(id == 98) return "Enemy Islands";
            if(id == 99) return "Far Out";
            if(id == 100) return "Front Line";
            if(id == 101) return "Inner Circle";
            if(id == 102) return "Motherland";
            if(id == 103) return "Open Plains";
            if(id == 104) return "Ring of Water";
            if(id == 105) return "Snakepit";
            if(id == 106) return "The Eye";
            if(id == 107) return "Australia";
            if(id == 108) return "Indochina";
            if(id == 109) return "Indonesia";
            if(id == 110) return "Strait of Malacca";
            if(id == 111) return "Philippines";
            if(id == 112) return "Bog Islands";
            if(id == 113) return "Mangrove Jungle";
            if(id == 114) return "Pacific Islands";
            if(id == 115) return "Sandbank";
            if(id == 116) return "Water Nomad";
            if(id == 117) return "Jungle Islands";
            if(id == 118) return "Holy Line";
            if(id == 119) return "Border Stones";
            if(id == 120) return "Yin Yang";
            if(id == 121) return "Jungle Lanes";
            if(id == 122) return "Alpine Lakes";
            if(id == 123) return "Bogland";
            if(id == 124) return "Mountain Ridge";
            if(id == 125) return "Ravines";
            if(id == 126) return "Wolf Hill";
            if(id == 132) return "Antarctica";
            if(id == 137) return "Custom Map Pool";
            if(id == 139) return "Golden Swamp";
            if(id == 140) return "Four Lakes";
            if(id == 141) return "Land Nomad";
            if(id == 142) return "Battle on Ice";
            if(id == 143) return "El Dorado";
            if(id == 144) return "Fall of Axum";
            if(id == 145) return "Fall of Rome";
            if(id == 146) return "Majapahit Empire";
            if(id == 147) return "Amazon Tunnel";
            if(id == 148) return "Coastal Forest";
            if(id == 149) return "African Clearing";
            if(id == 150) return "Atacama";
            if(id == 151) return "Seize the Mountain";
            if(id == 152) return "Crater";
            if(id == 153) return "Crossroads";
            if(id == 154) return "Michi";
            if(id == 155) return "Team Moats";
            if(id == 156) return "Volcanic Islan";
            return "Unknown";
        }
    }
}