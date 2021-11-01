using System.IO;
using System.Linq;

namespace ReadAoe2Recrod
{
    public partial class Aoe2Record
    {
        public string GameTypeName = "";
        public string GameTypeShort = "";
        
        private void ReadLobby(BinaryReader reader)
        {
            Padding(reader, 14);
            var teams = reader.ReadBytes(8);
            var revealMapId = reader.ReadUInt32();
            var fogOfWar = BoolUInt32(reader);
            var mapSize = reader.ReadUInt32();
            var population_limit_encoded = reader.ReadUInt32();
            var gameTypeId = reader.ReadByte();
            GameTypeName = ParseGameTypeName(gameTypeId);
            var lockTeams = reader.ReadBoolean();
            var treatyLength = reader.ReadByte();
            var cheatCodesUsed = reader.ReadUInt32();
            Padding(reader, 4);
            var numChat = reader.ReadUInt32();
            for (int i = 0; i < numChat; i++)
            {
                var messageLength = reader.ReadUInt32();
                var message = reader.ReadBytes( (int)(messageLength - (messageLength > 0 ? 1 : 0)));
                if(messageLength > 0) Padding(reader, 1);
            }
            var mapSeed = reader.ReadInt32();
            Padding(reader, 10);
        }

        private string ParseGameTypeName(int id)
        {
            if (id == 0) return "Random Map";
            if (id == 1) return "Regicide";
            if (id == 2) return "Deathmatch";
            if (id == 3) return "Scenario";
            if (id == 4) return "Campaign";
            if (id == 5) return "King of the Hill";
            if (id == 6) return "Wonder Race";
            if (id == 7) return "Defend the Wonder";
            if (id == 8) return "Turbo Random";
            if (id == 10) return "Capture the Relic";
            if (id == 11) return "Sudden Death";
            if (id == 12) return "Battle Royale";
            if (id == 13) return "Empire Wars";
            return "Unknown";
        }

        private string ParseGameTypeShort(int id)
        {
            if (id == 0) return "RM";
            if (id == 1) return "RE";
            if (id == 2) return "DM";
            if (id == 3) return "SC";
            if (id == 4) return "CP";
            if (id == 5) return "KH";
            if (id == 6) return "WR";
            if (id == 7) return "DWr";
            if (id == 8) return "TR";
            if (id == 10) return "CR";
            if (id == 11) return "SD";
            if (id == 12) return "BR";
            if (id == 13) return "EW";
            return "UN";
        }
    }
}