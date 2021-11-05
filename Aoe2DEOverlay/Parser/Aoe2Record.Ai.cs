using System.IO;
using System.Linq;

namespace ReadAoe2Recrod
{
    public partial class Aoe2Record
    {
        public bool HasAI = false;
        
        private void ReadAi(BinaryReader reader)
        {
            HasAI = BoolUInt32(reader);
            if (HasAI)
            {  
                // For now we simply skip it
                byte[] pattern = Enumerable.Repeat((byte)0x00, 4096).ToArray();
                Find(reader,  pattern);
            }
        }
    }
}