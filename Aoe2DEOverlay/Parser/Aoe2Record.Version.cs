using System;
using System.IO;

namespace ReadAoe2Recrod
{
    public partial class Aoe2Record
    {
        public string GameVersion;
        public double SaveVersion;
        public bool IsDE = false;
        public double Version;
        
        private void ReadVersion(BinaryReader reader)
        {
            GameVersion = CString(reader);
            SaveVersion = Math.Round(reader.ReadSingle(), 2);
            IsDE = GameVersion == "VER 9.4" && SaveVersion >= 12.97;
            if(!IsDE) return;
            var build= reader.ReadUInt32();
            var timestamp= reader.ReadUInt32();
            Version = reader.ReadSingle();
            var intervalVersion = reader.ReadUInt32();
            var gameOptionsVersion = reader.ReadUInt32();
            var dlcCount = reader.ReadUInt32();
            var dlcIds = ArrayUInt32(reader, (int)dlcCount);
        }
    }
}