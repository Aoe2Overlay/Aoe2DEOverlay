using System;
using System.IO;
using System.Text;

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
            ReadLobby(reader);
            reader.Close();
        }
        
        public void SkipToEndOfScenario(BinaryReader reader)
        {
            // skip to end of game settings at scenario
            // convert double to hex 3.0 = 0x4008000000000000 (result is inverted) https://gregstoll.com/~gregstoll/floattohex/ 
            var pattern = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08, 0x40 }; // double 3.0 
           if (!Find(reader, pattern))
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