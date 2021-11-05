using System.IO;

namespace ReadAoe2Recrod
{
    public partial class Aoe2Record
    {
        
        private void ReadAchievements(BinaryReader reader)
        {
            Padding(reader, 13);
            var totalPoints = reader.ReadUInt32();
            Padding(reader, 26);
            var warPoints = reader.ReadUInt32();
            Padding(reader, 34);
            var numKills = reader.ReadSingle();
            Padding(reader, 28);
            var numkilled = reader.ReadSingle();
            Padding(reader, 28);
            var numkilled2 = reader.ReadSingle();
            Padding(reader, 284);
            Padding(reader, 4);
            Padding(reader, 60);
            Padding(reader, 4);
            Padding(reader, 444);
            var totalFood = reader.ReadSingle();
            Padding(reader, 28);
            var totalWood = reader.ReadSingle();
            Padding(reader, 28);
            var totalStone = reader.ReadSingle();
            Padding(reader, 28);
            var totalGold = reader.ReadSingle();
            Padding(reader, 444);
            var maxVillagers = reader.ReadSingle();
            Padding(reader, 28);
            var maxVillagers2 = reader.ReadSingle();
            Padding(reader, 28);
            var relicCastle = reader.ReadSingle();
            Padding(reader, 28);
            var relicCastle2 = reader.ReadSingle();
            Padding(reader, 28);
            var wonder = reader.ReadSingle();
            Padding(reader, 28);
            var relicGold = reader.ReadSingle();
            Padding(reader, 124);
            var researchedTechs = reader.ReadSingle();
            Padding(reader, 28);
            var exploredPercent = reader.ReadSingle();
            Padding(reader, 4);
        }
    }
}