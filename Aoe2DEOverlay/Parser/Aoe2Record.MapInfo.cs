using System.IO;

namespace ReadAoe2Recrod
{
    public partial class Aoe2Record
    {
        
        public void ReadMapInfo(BinaryReader reader)
        {
            var sizeX = reader.ReadUInt32();
            var sizeY = reader.ReadUInt32();
            var tileNum = sizeX * sizeY;
            var zoneNum = reader.ReadUInt32();
            for (int i = 0; i < zoneNum; i++)
            {
                Padding(reader, (int)(2048 + tileNum * 2));
                var numFloats = reader.ReadUInt32();
                Padding(reader, (int)(numFloats * 4));
                Padding(reader, 4);
            }
            var allVisible = reader.ReadBoolean();
            var fogOfWar = reader.ReadBoolean();
            //  DE 12.97 fix START 
            /*
            var peekStart = reader.BaseStream.Position;
            Padding(reader, (int)tileNum * 7);
            var val = reader.ReadUInt32(); 
            var peekEnd = reader.BaseStream.Position;
            reader.BaseStream.Seek(peekStart - peekEnd, SeekOrigin.Current);
            */
            // DE fix END
            for (int i = 0; i < tileNum; i++)
            {
                var terrainType = reader.ReadByte();
                Padding(reader, 1);
                var elevation = reader.ReadByte();
                var unk0 = reader.ReadInt16();
                var unk1 = reader.ReadInt16();
                var unk2 = reader.ReadInt16();
            }
            var numData = reader.ReadUInt32();
            Padding(reader, 4);
            Padding(reader, 4 * (int)numData);
            for (int i = 0; i < numData; i++)
            {
                var numObstructions = reader.ReadUInt32();
                Padding(reader, 8 * (int)numObstructions);
            }
            var sizeX2 = reader.ReadUInt32();
            var sizeY2 = reader.ReadUInt32();
            Padding(reader, (int)tileNum * 4); // visibility
        } 
    }
}