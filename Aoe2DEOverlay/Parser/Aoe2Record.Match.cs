using System;
using System.IO;
using System.Linq;

namespace ReadAoe2Recrod
{
    public partial class Aoe2Record
    {
        public string Difficulty;
        public string MapName;
        public int MapType;
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
            MapType = (int) resolvedMapId;
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
            Padding(reader, 9);
            var fogOfWar = reader.ReadBoolean();
            var cheatNotifications = reader.ReadBoolean();
            var coloredChat = reader.ReadBoolean();
            Padding(reader, 4); // separator
            IsRanked = reader.ReadBoolean();
            var allowSpecs = reader.ReadBoolean();
            var lobbyVisibility = reader.ReadUInt32();
            var hiddenCivs = reader.ReadBoolean();
            var matchmaking = reader.ReadBoolean();
            var specDelay = reader.ReadUInt32();
            var scenarioCiv = reader.ReadByte();
            var rmsCrc = reader.ReadBytes(4);
            
            var length = 23;
            var strings = new string[length];
            for (int i = 0; i < length; i++)
            {
                strings[i] = DEString(reader);
                while (new uint[] {3, 21, 23, 42, 44, 45, 46, 47}.Contains(reader.ReadUInt32())){}
            }

            var num_ai_files = (int)reader.ReadUInt64();
            for (int i = 0; i < num_ai_files; i++)
            {
                Padding(reader, 4);
                DEString(reader);
                Padding(reader, 4);
            }
            Padding(reader, 8);
            var guid = reader.ReadBytes(16);
            var lobbyName = DEString(reader);
            Padding(reader, 8);
            var moddedDataset = DEString(reader);
            Padding(reader, 19);
            Padding(reader, 5);
            Padding(reader, 9);
            Padding(reader, 1);
            Padding(reader, 8);
            Padding(reader, 21);
            Padding(reader, 4);
            DEString(reader);
            Padding(reader, 5);
            Padding(reader, 1);
            Padding(reader, 2);
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
            return Aoe2Mapper.ParseMapName((int)id);
        }
    }
}