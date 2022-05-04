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
            Padding(reader, 5); // >= 26.16
            var revealMapId = reader.ReadUInt32();
            var fogOfWar = BoolUInt32(reader);
            var mapSize = reader.ReadUInt32();
            var populationLimitEncoded = reader.ReadUInt32();
            var gameTypeId = reader.ReadByte();
            GameTypeName = ParseGameTypeName(gameTypeId);
            GameTypeShort = ParseGameTypeShort(gameTypeId);
            var lockTeams = reader.ReadBoolean();
            var treatyLength = reader.ReadByte();
            var cheatCodesUsed = reader.ReadUInt32();
            Padding(reader, 4);
            Padding(reader, 1);  // since aoe2 patch January 31 2022 (Update 58259)
            var numChat = reader.ReadUInt32();
            for (int i = 0; i < numChat; i++)
            {
                var messageLength = reader.ReadUInt32();
                var message = reader.ReadBytes( (int)(messageLength - (messageLength > 0 ? 1 : 0)));
                if(messageLength > 0) Padding(reader, 1);
            }
            var mapSeed = reader.ReadInt32();
            Padding(reader, 10);
            Padding(reader, 4); // >= 26.16
        }

        private string ParseGameTypeName(int id)
        {
            return Aoe2Mapper.ParseGameTypeName(id);
        }

        private string ParseGameTypeShort(int id)
        {
            return Aoe2Mapper.ParseGameTypeShort(id);
        }
    }
}