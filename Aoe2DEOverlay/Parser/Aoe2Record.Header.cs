using System.IO;
using System.Text;

namespace ReadAoe2Recrod
{
    public partial class Aoe2Record
    {
        private BinaryReader DecompresseHeader(BinaryReader reader)
        {
            var headerLength = reader.ReadUInt32();
            var check = PeekUInt32(reader);
            var chapterAddress = check < 100000000 ? reader.ReadUInt32() : 0;
            var compressedLength = (int)(headerLength - 4 - (check < 100000000 ? 4 : 0));
            var compresseHeader = reader.ReadBytes(compressedLength);
            var decompresseHeader = Decompress(compresseHeader);
            reader.Close();
            return new BinaryReader(new MemoryStream(decompresseHeader), Encoding.ASCII);
        }
    }
}