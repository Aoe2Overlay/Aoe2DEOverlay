using System.IO;

namespace ReadAoe2Recrod
{
    public partial class Aoe2Record
    {
        private void ReadScenario(BinaryReader reader)
        {
            // scenario header
            var nextUid = reader.ReadUInt32();
            var constant = reader.ReadBytes(4);
            var names = ArrayString(reader, 16, 256);
            var playerIds = ArrayUInt32(reader, 16);
            for (int i = 0; i < 16; i++)
            {
                var active = reader.ReadUInt32();
                var human = reader.ReadUInt32();
                var civilization = reader.ReadUInt32();
                var playerConstant = reader.ReadUInt32(); //  0x04 0x00 0x00 0x00
            }
            Padding(reader, 5);
            var elapsedTime = reader.ReadSingle();
            var scenarioFilename = PascalString16(reader);
            Padding(reader, 64);
            Padding(reader, 64);
            
            // Scenarios intro text, a bitmap, and cinematics.
            var instructionId = reader.ReadUInt32();
            var hintsId = reader.ReadUInt32();
            var victoryId = reader.ReadUInt32();
            var defeatId = reader.ReadUInt32();
            var historyId = reader.ReadUInt32();
            var scoutsId = reader.ReadUInt32();
            var instructionsLength = reader.ReadUInt16();
            var instructions = reader.ReadBytes(instructionsLength);
            var hint = PascalString16(reader);
            var victory = PascalString16(reader);
            var defeat = PascalString16(reader);
            var history = PascalString16(reader);
            var scouts = PascalString16(reader);
            var pgCin = PascalString16(reader);
            var victCin = PascalString16(reader);
            var lossCin = PascalString16(reader);
            var background = PascalString16(reader);
            var bitmapIncluded = reader.ReadUInt32();
            var bitmapX = reader.ReadUInt32();
            var bitmapY = reader.ReadUInt32();
            Padding(reader, 2);
            Padding(reader, 64); // 16 nulls
            
            // Scenario player definitions.
            var num = 16;
            var aiNames = new string[num];
            for (int i = 0; i < num; i++) PascalString16(reader);
            for (int i = 0; i < num; i++)
            {
                Padding(reader, 8);
                var file = PascalString32(reader);
            }
            Padding(reader, 4);
            for (int i = 0; i < num; i++)
            {
                var gold = reader.ReadUInt32();
                var wood = reader.ReadUInt32();
                var food = reader.ReadUInt32();
                var stone = reader.ReadUInt32();
                var unk0 = reader.ReadUInt32();
                var unk1 = reader.ReadUInt32();
                var unk2 = reader.ReadUInt32();
            }
            Padding(reader, num * 1);
            
            // Scenario Victory conditions
            Padding(reader, 4);
            var isConquest = reader.ReadUInt32();
            Padding(reader, 4);
            var relics = reader.ReadUInt32();
            Padding(reader, 4);
            var explored = reader.ReadUInt32();
            Padding(reader, 4);
            var all = reader.ReadUInt32();
            var mode = reader.ReadUInt32();
            var score = reader.ReadUInt32();
            var time = reader.ReadUInt32();
            
            // Unknown
            Padding(reader, 12544); // unknown
            
            // Disabled techs, units, and buildings.
            Padding(reader, 4);
            Padding(reader, 64);
            Padding(reader, 196);
            Padding(reader, 12);
            
            // Game settings
            var startingAges = ArrayInt32(reader, 16);
            Padding(reader, 4);
            Padding(reader, 8);
            var mapId = reader.ReadUInt32();
            var difficultyId = reader.ReadUInt32();
            var lockTeams = reader.ReadUInt32();
            Padding(reader, 167);
            for (int i = 0; i < 9; i++)
            {
                var dataRef = reader.ReadUInt32();
                var playerTypeId = reader.ReadUInt32();
                var name = PascalString32(reader);
            }
            Padding(reader, 36);
            Padding(reader, 4);
            Find(reader, new byte []{ 0x33,0x33,0x33,0x33,0x33,0x33,0x03,0x40});
            
            // Triggers
            Padding(reader, 1);
            var numTriggers = reader.ReadUInt32();
            Padding(reader, 1032);
        }
    }
}