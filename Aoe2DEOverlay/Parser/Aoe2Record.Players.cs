using System;
using System.Collections.Generic;
using System.IO;

namespace ReadAoe2Recrod
{
    public partial class Aoe2Record
    {
        public RecordPlayer[] Players = Array.Empty<RecordPlayer>();
        
        private void ReadPlayers(BinaryReader reader)
        {
            var players = new List<RecordPlayer>(8); 
            for (int i = 0; i < 8; i++)
            {
                var player = new RecordPlayer();
                var dlcId  = reader.ReadUInt32();
                var colorId = reader.ReadInt32();
                player.Color = colorId + 1;
                var selectedColor  = reader.ReadByte();
                var selectedTeamId  = reader.ReadByte();
                var resolvedTeamId  = reader.ReadByte();
                var datCrc = reader.ReadBytes(8);
                var mpGameVersion  = reader.ReadByte();
                var civId  = reader.ReadUInt32();
                player.Civ = ParseCiv(civId);
                var aiType = DEString(reader);
                var aiCiv_name_index = reader.ReadByte();
                var aiName = DEString(reader);
                var name = DEString(reader);
                player.Name = name.Length > 0 ? name : aiName;
                var typeId = reader.ReadUInt32();
                var type = ParseType(typeId);
                player.ProfileId = reader.ReadUInt32();
                Padding(reader, 4);
                player.Slot = reader.ReadInt32();
                
                Padding(reader, 10);  // since aoe2 patch January 31 2022 (Update 58259)
                
                if(typeId != 1 /* is not closed */ ) players.Add(player);
            }
            Players = players.ToArray();
        }
        public string ParseType(uint id)
        {
            if (id == 0) return "Absent";
            if (id == 1) return "Closed";
            if (id == 2) return "Human";
            if (id == 3) return "Eliminated";
            if (id == 4) return "Computer";
            if (id == 5) return "Cyborg";
            if (id == 6) return "Spectator";
            return "Unknown";
        }
        
        public string ParseCiv(uint id)
        {
            return Aoe2Mapper.ParseCiv(id);
        }
    }

    public class RecordPlayer
    {
        public int Slot;
        public int Color;
        public string Name;
        public string Civ;
        public uint ProfileId;
        public bool IsAi => ProfileId == 0;

    }
}