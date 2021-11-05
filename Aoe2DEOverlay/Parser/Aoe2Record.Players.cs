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
            Padding(reader, 4); // separator
            
            var players = new List<RecordPlayer>(8); 
            for (int i = 0; i < 8; i++)
            {
                var player = new RecordPlayer();
            
                var dlc_id  = reader.ReadUInt32();
                var colorId = reader.ReadInt32();
                player.Color = colorId + 1;
                var selected_color  = reader.ReadByte();
                var selected_team_id  = reader.ReadByte();
                var resolved_team_id  = reader.ReadByte();
                var dat_crc = reader.ReadBytes(8);
                var mp_game_version  = reader.ReadByte();
                var civId  = reader.ReadUInt32();
                player.Civ = ParseCiv(civId);
                var ai_type = DEString(reader);
                var ai_civ_name_index = reader.ReadByte();
                var ai_name = DEString(reader);
                var name = DEString(reader);
                player.Name = name.Length > 0 ? name : ai_name;
                var type_id = reader.ReadUInt32();
                var type = ParseType(type_id);
                player.ProfileId = reader.ReadUInt32();
                Padding(reader, 4);
                player.Slot = reader.ReadInt32();
                var hd_rm_elo  = reader.ReadUInt32();
                var hd_dm_elo  = reader.ReadUInt32();
                var animated_destruction_enabled  = reader.ReadBoolean();
                var custom_ai  = reader.ReadBoolean();
                
                if(type_id != 1 /* is not closed */ ) players.Add(player);
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