using System;
using System.IO;
namespace ReadAoe2Recrod
{
    public partial class Aoe2Record
    {
        public uint Started = 0;
        public void Read(BinaryReader reader)
        {
            reader = DecompresseHeader(reader);
            ReadVersion(reader);
            ReadMatchPart1(reader);
            ReadPlayers(reader);
            ReadMatchPart2(reader);
            ReadAi(reader);
            ReadReplay(reader);
            ReadMapInfo(reader);
            SkipToEndOfScenario(reader);
            //ReadInitial(reader);
            //ReadAchievements(reader);
            //ReadScenario(reader);
            ReadLobby(reader);
            reader.Close();
        }


        public void SkipToEndOfScenario(BinaryReader reader)
        {
            // skip to end of game settings at scenario
            var pattern = new byte[] { 0x33, 0x33, 0x33, 0x33, 0x33, 0x33, 0x03, 0x40 };
            Find(reader, pattern);
            if (reader.BaseStream.Length == reader.BaseStream.Position)
            {
                throw new Exception("ERROR: scenario end not found!");
            }
            // skip triggers of scenario
            Padding(reader, 1);
            Padding(reader, 4);// var numOTriggers = reader.ReadUInt32();
            Padding(reader, 1032);
        }
    }
}