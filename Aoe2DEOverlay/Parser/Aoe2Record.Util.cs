using System;
using System.IO;
using System.IO.Compression;

using System.Text;
namespace ReadAoe2Recrod
{
    public partial class Aoe2Record
    {
        public static UInt32 PeekUInt32(BinaryReader reader)
        {
            var pos = reader.BaseStream.Position;
            var value = reader.ReadUInt32();
            var offset = reader.BaseStream.Position - pos;
            reader.BaseStream.Seek(-offset, SeekOrigin.Current);
            return value;

        }

        public static string CString(BinaryReader reader)
        {
            var builder = new StringBuilder();
            var character = reader.ReadChar();
            while (character != '\0')
            {
                builder.Append(character);
                character = reader.ReadChar();
            }
            return builder.ToString();
        }

        public static string DEString(BinaryReader reader)
        {
            var b1 = reader.ReadByte();
            var b2 = reader.ReadByte();

            if (b1 == 0x60 && b2 == 0x0A)
            {
                var len = reader.ReadUInt16();
                var bytes = reader.ReadBytes(len);
                return Encoding.UTF8.GetString(bytes);
            }

            throw new Exception("ERROR: is not a DEString!");
        }

        public static void Const(BinaryReader reader, byte[] pattern)
        {
            foreach (var b in pattern)
            {
                if (reader.ReadByte() != b)
                {
                    throw new Exception($"ERROR: is not a Const {Encoding.UTF8.GetString(pattern)}!");
                }
            }
        }

        public static string PascalString16(BinaryReader reader)
        {
            var size = reader.ReadInt16();
            var bytes = reader.ReadBytes(size);
            return Encoding.UTF8.GetString(bytes);
        }

        public static string PascalString32(BinaryReader reader)
        {
            var size = reader.ReadInt32();
            var bytes = reader.ReadBytes(size);
            return Encoding.UTF8.GetString(bytes);
        }

        public static bool BoolUInt32(BinaryReader reader)
        {
            return reader.ReadUInt32() > 0;
        }

        public static byte[] ArrayByte(BinaryReader reader, int count)
        {
            var array = new byte[count];
            for (int i = 0; i < count; i++) array[i] = reader.ReadByte();
            return array;
        }

        public static float[] ArrayFloat32(BinaryReader reader, int count)
        {
            var array = new float[count];
            for (int i = 0; i < count; i++) array[i] = reader.ReadSingle();
            return array;
        }

        public static uint[] ArrayUInt32(BinaryReader reader, int count)
        {
            var array = new uint[count];
            for (int i = 0; i < count; i++) array[i] = reader.ReadUInt32();
            return array;
        }

        public static int[] ArrayInt32(BinaryReader reader, int count)
        {
            var array = new int[count];
            for (int i = 0; i < count; i++) array[i] = reader.ReadInt32();
            return array;
        }

        public static string[] ArrayString(BinaryReader reader, int count, int len)
        {
            var array = new string[count];
            for (int i = 0; i < count; i++) array[i] = Encoding.UTF8.GetString(reader.ReadBytes(len));;
            return array;
        }

        public static void Find(BinaryReader reader, byte[] pattern, int offset=0)
        {
            var i = 0;
            var match = false;
            while (!match && reader.BaseStream.Position != reader.BaseStream.Length)
            {
                if (reader.ReadChar() == pattern[i])
                {
                    i += 1;
                    if (i == pattern.Length)
                    {
                        match = true;
                    }
                }
                else 
                {
                    if(i > 1) reader.BaseStream.Seek(i - 1, SeekOrigin.Current);
                    i = 0;
                }
            }

            if (offset != 0)
            {
                reader.BaseStream.Seek(offset, SeekOrigin.Current);
            }
        }

        public static void Padding(BinaryReader reader, int number)
        {
            reader.BaseStream.Seek(number, SeekOrigin.Current);
            //reader.ReadBytes(number);
        }

        public static byte[] Decompress(byte[] data)
        {
            using (var compressedStream = new MemoryStream(data))
            using (var zipStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                zipStream.CopyTo(resultStream);
                return resultStream.ToArray();
            }
        }
    }
}