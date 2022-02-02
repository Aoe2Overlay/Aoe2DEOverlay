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
            Padding(reader, 8);
            Padding(reader, 4); // separatorosition;
            
            Padding(reader, 33); // since aoe2 patch January 31 2022 (Update 58259)
            
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
            if (id == 1) return "Britons";
            if (id == 2) return "Franks";
            if (id == 3) return "Goths";
            if (id == 4) return "Teutons";
            if (id == 5) return "Japanese";
            if (id == 6) return "Chinese";
            if (id == 7) return "Byzantines";
            if (id == 8) return "Persians";
            if (id == 9) return "Saracens";
            if (id == 10) return "Turks";
            if (id == 11) return "Vikings";
            if (id == 12) return "Mongols";
            if (id == 13) return "Celts";
            if (id == 14) return "Spanish";
            if (id == 15) return "Aztecs";
            if (id == 16) return "Mayans";
            if (id == 17) return "Huns";
            if (id == 18) return "Koreans";
            if (id == 19) return "Italians";
            if (id == 20) return "Indians";
            if (id == 21) return "Incas";
            if (id == 22) return "Magyars";
            if (id == 23) return "Slavs";
            if (id == 24) return "Portuguese";
            if (id == 25) return "Ethiopians";
            if (id == 26) return "Malians";
            if (id == 27) return "Berbers";
            if (id == 28) return "Khmer";
            if (id == 29) return "Malay";
            if (id == 30) return "Burmese";
            if (id == 31) return "Vietnamese";
            if (id == 32) return "Bulgarians";
            if (id == 33) return "Tatars";
            if (id == 34) return "Cumans";
            if (id == 35) return "Lithuanians";
            if (id == 36) return "Burgundians";
            if (id == 37) return "Sicilians";
            if (id == 38) return "Poles";
            if (id == 39) return "Bohemian";
            return "Unknown";
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