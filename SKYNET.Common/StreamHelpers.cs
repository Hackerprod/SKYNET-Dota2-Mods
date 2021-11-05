using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

public static class StreamHelpers
{
    public static string ReadNullTermString(this BinaryReader stream, Encoding encoding)
    {
        int byteCount = encoding.GetByteCount("e");
        using (MemoryStream memoryStream = new MemoryStream())
        {
            while (true)
            {
                byte[] array = new byte[byteCount];
                stream.Read(array, 0, byteCount);
                if (encoding.GetString(array, 0, byteCount) == "\0")
                {
                    break;
                }
                memoryStream.Write(array, 0, array.Length);
            }
            return encoding.GetString(memoryStream.ToArray());
        }
    }

    private static byte Short1(short x)
    {
        return (byte)(x >> 8);
    }

    private static byte Short0(short x)
    {
        return (byte)x;
    }

    private static byte Int3(int x)
    {
        return (byte)(x >> 24);
    }

    private static byte Int2(int x)
    {
        return (byte)(x >> 16);
    }

    private static byte Int1(int x)
    {
        return (byte)(x >> 8);
    }

    private static byte Int0(int x)
    {
        return (byte)x;
    }

    private static byte Long7(long x)
    {
        return (byte)(x >> 56);
    }

    private static byte Long6(long x)
    {
        return (byte)(x >> 48);
    }

    private static byte Long5(long x)
    {
        return (byte)(x >> 40);
    }

    private static byte Long4(long x)
    {
        return (byte)(x >> 32);
    }

    private static byte Long3(long x)
    {
        return (byte)(x >> 24);
    }

    private static byte Long2(long x)
    {
        return (byte)(x >> 16);
    }

    private static byte Long1(long x)
    {
        return (byte)(x >> 8);
    }

    private static byte Long0(long x)
    {
        return (byte)x;
    }

    private static short MakeInt16L(byte b0, byte b1)
    {
        return (short)((b1 << 8) | (b0 & 0xFF));
    }

    private static short MakeInt16B(byte b1, byte b0)
    {
        return MakeInt16L(b1, b0);
    }

    private static int MakeInt32L(byte b0, byte b1, byte b2, byte b3)
    {
        return ((b3 & 0xFF) << 24) | ((b2 & 0xFF) << 16) | ((b1 & 0xFF) << 8) | (b0 & 0xFF);
    }

    private static int MakeInt32B(byte b3, byte b2, byte b1, byte b0)
    {
        return MakeInt32L(b3, b2, b1, b0);
    }

    private static long MakeInt64L(byte b0, byte b1, byte b2, byte b3, byte b4, byte b5, byte b6, byte b7)
    {
        return ((b7 & 0xFF) << 24) | ((b6 & 0xFF) << 16) | ((b5 & 0xFF) << 8) | (b4 & 0xFF) | ((b3 & 0xFF) << 24) | ((b2 & 0xFF) << 16) | ((b1 & 0xFF) << 8) | (b0 & 0xFF);
    }

    private static long MakeInt64B(byte b7, byte b6, byte b5, byte b4, byte b3, byte b2, byte b1, byte b0)
    {
        return MakeInt64L(b7, b6, b5, b4, b3, b2, b1, b0);
    }

    public static short ReadInt16L(this Stream stream)
    {
        return MakeInt16L((byte)stream.ReadByte(), (byte)stream.ReadByte());
    }

    public static ushort ReadUInt16L(this Stream stream)
    {
        return (ushort)stream.ReadInt16L();
    }

    public static short ReadInt16B(this Stream stream)
    {
        return MakeInt16B((byte)stream.ReadByte(), (byte)stream.ReadByte());
    }

    public static ushort ReadUInt16B(this Stream stream)
    {
        return (ushort)stream.ReadInt16B();
    }

    public static void WriteInt16L(this Stream stream, short v)
    {
        stream.WriteByte(Short0(v));
        stream.WriteByte(Short1(v));
    }

    public static void WriteUInt16L(this Stream stream, ushort v)
    {
        stream.WriteInt16L((short)v);
    }

    public static void WriteInt16B(this Stream stream, short v)
    {
        stream.WriteByte(Short1(v));
        stream.WriteByte(Short0(v));
    }

    public static void WriteUInt16B(this Stream stream, ushort v)
    {
        stream.WriteInt16B((short)v);
    }

    public static int ReadInt32L(this Stream stream)
    {
        return MakeInt32L((byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte());
    }

    public static uint ReadUInt32L(this Stream stream)
    {
        return (uint)stream.ReadInt32L();
    }

    public static int ReadInt32B(this Stream stream)
    {
        return MakeInt32B((byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte());
    }

    public static uint ReadUInt32B(this Stream stream)
    {
        return (uint)stream.ReadInt32B();
    }

    public static void WriteInt32L(this Stream stream, int v)
    {
        stream.WriteByte(Int0(v));
        stream.WriteByte(Int1(v));
        stream.WriteByte(Int2(v));
        stream.WriteByte(Int3(v));
    }

    public static void WriteBytes(this Stream stream, byte[] buffer, int offset, int length)
    {
        stream.Write(buffer, offset, length);
    }

    public static void WriteUInt32L(this Stream stream, uint v)
    {
        stream.WriteInt32L((int)v);
    }

    public static void WriteInt32B(this Stream stream, int v)
    {
        stream.WriteByte(Int3(v));
        stream.WriteByte(Int2(v));
        stream.WriteByte(Int1(v));
        stream.WriteByte(Int0(v));
    }

    public static void WriteUInt32B(this Stream stream, uint v)
    {
        stream.WriteInt32B((int)v);
    }

    public static long ReadInt64L(this Stream stream)
    {
        return MakeInt64L((byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte());
    }

    public static ulong ReadUInt64L(this Stream stream)
    {
        return (ulong)stream.ReadInt64L();
    }

    public static long ReadInt64B(this Stream stream)
    {
        return MakeInt64B((byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte(), (byte)stream.ReadByte());
    }

    public static ulong ReadUInt64B(this Stream stream)
    {
        return (ulong)stream.ReadInt64B();
    }

    public static void WriteInt64L(this Stream stream, long v)
    {
        stream.WriteByte(Long0(v));
        stream.WriteByte(Long1(v));
        stream.WriteByte(Long2(v));
        stream.WriteByte(Long3(v));
        stream.WriteByte(Long4(v));
        stream.WriteByte(Long5(v));
        stream.WriteByte(Long6(v));
        stream.WriteByte(Long7(v));
    }

    public static void WriteUInt64L(this Stream stream, ulong v)
    {
        stream.WriteInt64L((long)v);
    }

    public static void WriteInt64B(this Stream stream, long v)
    {
        stream.WriteByte(Long7(v));
        stream.WriteByte(Long6(v));
        stream.WriteByte(Long5(v));
        stream.WriteByte(Long4(v));
        stream.WriteByte(Long3(v));
        stream.WriteByte(Long2(v));
        stream.WriteByte(Long1(v));
        stream.WriteByte(Long0(v));
    }

    public static void WriteUInt64B(this Stream stream, ulong v)
    {
        stream.WriteInt64B((long)v);
    }

    private static int SingleToInt32Bits(float f)
    {
        return BitConverter.ToInt32(BitConverter.GetBytes(f), 0);
    }

    private static float Int32BitsToSingle(int i)
    {
        return BitConverter.ToSingle(BitConverter.GetBytes(i), 0);
    }

    public static float ReadFloatL(this Stream stream)
    {
        return Int32BitsToSingle(stream.ReadInt32L());
    }

    public static float ReadFloatB(this Stream stream)
    {
        return Int32BitsToSingle(stream.ReadInt32B());
    }

    public static void WriteFloatL(this Stream stream, float v)
    {
        stream.WriteInt32L(SingleToInt32Bits(v));
    }

    public static void WriteFloatB(this Stream stream, float v)
    {
        stream.WriteInt32B(SingleToInt32Bits(v));
    }

    public static double ReadDoubleL(this Stream stream)
    {
        return BitConverter.Int64BitsToDouble(stream.ReadInt64L());
    }

    public static double GetDoubleB(this Stream stream)
    {
        return BitConverter.Int64BitsToDouble(stream.ReadInt64B());
    }

    public static void WriteDoubleL(this Stream stream, double v)
    {
        stream.WriteInt64L(BitConverter.DoubleToInt64Bits(v));
    }

    public static void WriteDoubleB(this Stream stream, double v)
    {
        stream.WriteInt64B(BitConverter.DoubleToInt64Bits(v));
    }

    public static byte[] ReadBytes(this Stream stream, int count)
    {
        byte[] array = new byte[count];
        stream.Read(array, 0, count);
        return array;
    }

    public static void WriteBytes(this Stream stream, byte[] buffer)
    {
        stream.Write(buffer, 0, buffer.Length);
    }

    public static string ReadWideString(this Stream stream)
    {
        StringBuilder stringBuilder = new StringBuilder();
        char c = (char)stream.ReadUInt16L();
        while (true)
        {
            switch (c)
            {
                case '\v':
                    stringBuilder.Append("\\v");
                    break;
                default:
                    stringBuilder.Append(c);
                    break;
                case '\0':
                    return stringBuilder.ToString();
            }
            c = (char)stream.ReadUInt16L();
        }
    }

    public static void WriteWideString(this Stream stream, string str, Encoding encoding)
    {
        string text = str.Replace("\\v", "\v");
        if (!text.EndsWith("\0"))
        {
            text += "\0";
        }
        byte[] bytes = encoding.GetBytes(text);
        stream.Write(bytes, 0, bytes.Length);
    }

    public static string ReadNullTermString(this Stream stream, Encoding encoding)
    {
        int byteCount = encoding.GetByteCount("e");
        using (MemoryStream memoryStream = new MemoryStream())
        {
            while (true)
            {
                byte[] array = new byte[byteCount];
                stream.Read(array, 0, byteCount);
                if (encoding.GetString(array, 0, byteCount) == "\0")
                {
                    break;
                }
                memoryStream.Write(array, 0, array.Length);
            }
            return encoding.GetString(memoryStream.ToArray());
        }
    }

    public static void WriteNullTermString(this Stream stream, string value, Encoding encoding)
    {
        value = (value ?? string.Empty);
        int byteCount = encoding.GetByteCount(value);
        byte[] array = new byte[byteCount + 1];
        encoding.GetBytes(value, 0, value.Length, array, 0);
        array[byteCount] = 0;
        stream.Write(array, 0, array.Length);
    }

    public static async Task<byte[]> ReadAllBytesAsync(string filename)
    {
        byte[] buffer = new byte[4096];
        using (MemoryStream ms = new MemoryStream())
        {
            using (FileStream fs = File.OpenRead(filename))
            {
                while (true)
                {
                    TaskAwaiter<int> taskAwaiter4 = fs.ReadAsync(buffer, 0, buffer.Length).GetAwaiter();
                    if (!taskAwaiter4.IsCompleted)
                    {
                        taskAwaiter4 = default(TaskAwaiter<int>);
                    }
                    int result2;
                    int result = result2 = taskAwaiter4.GetResult();
                    if (result2 <= 0)
                    {
                        break;
                    }
                    TaskAwaiter taskAwaiter3 = ms.WriteAsync(buffer, 0, result).GetAwaiter();
                    if (!taskAwaiter3.IsCompleted)
                    {
                        taskAwaiter3 = default(TaskAwaiter);
                    }
                    taskAwaiter3.GetResult();
                }
            }
            return ms.ToArray();
        }
    }

    public static async Task WriteAllBytesAsync(string filename, byte[] bytes)
    {
        using (FileStream fs = File.Open(filename, FileMode.Create))
        {
            await fs.WriteAsync(bytes, 0, bytes.Length);
        }
    }

    public static async Task<string> ReadAllTextAsync(string filename)
    {
        Encoding uTF = Encoding.UTF8;
        Encoding encoding = uTF;
        return encoding.GetString(await ReadAllBytesAsync(filename).ConfigureAwait(continueOnCapturedContext: false));
    }

































    private static readonly byte[] data = new byte[8];

    private static byte[] bufferCache;

    private static readonly byte[] discardBuffer = new byte[8192];

    public static bool Available(this Stream stream)
    {
        return stream.Position < stream.Length;
    }


    public static short ReadInt16(this Stream stream, bool isBigEndian = false)
    {
        stream.Read(data, 0, 2);
        if (isBigEndian)
        {
            Array.Reverse(data, 0, 2);
        }
        return BitConverter.ToInt16(data, 0);
    }

    public static void WriteInt16(this Stream stream, short value, bool isBigEndian = false)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        if (isBigEndian)
        {
            Array.Reverse(bytes);
        }
        stream.Write(bytes, 0, bytes.Length);
    }

    public static ushort ReadUInt16(this Stream stream, bool isBigEndian = false)
    {
        stream.Read(data, 0, 2);
        if (isBigEndian)
        {
            Array.Reverse(data, 0, 2);
        }
        return BitConverter.ToUInt16(data, 0);
    }

    public static void WriteUInt16(this Stream stream, ushort value, bool isBigEndian = false)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        if (isBigEndian)
        {
            Array.Reverse(bytes);
        }
        stream.Write(bytes, 0, bytes.Length);
    }

    public static int ReadInt32(this Stream stream, bool isBigEndian = false)
    {
        stream.Read(data, 0, 4);
        if (isBigEndian)
        {
            Array.Reverse(data, 0, 4);
        }
        return BitConverter.ToInt32(data, 0);
    }

    public static void WriteInt32(this Stream stream, int value, bool isBigEndian = false)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        if (isBigEndian)
        {
            Array.Reverse(bytes);
        }
        stream.Write(bytes, 0, bytes.Length);
    }

    public static uint ReadUInt32(this Stream stream, bool isBigEndian = false)
    {
        stream.Read(data, 0, 4);
        if (isBigEndian)
        {
            Array.Reverse(data, 0, 4);
        }
        return BitConverter.ToUInt32(data, 0);
    }

    public static void WriteUInt32(this Stream stream, uint value, bool isBigEndian = false)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        if (isBigEndian)
        {
            Array.Reverse(bytes);
        }
        stream.Write(bytes, 0, bytes.Length);
    }

    public static long ReadInt64(this Stream stream, bool isBigEndian = false)
    {
        stream.Read(data, 0, 8);
        if (isBigEndian)
        {
            Array.Reverse(data, 0, 8);
        }
        return BitConverter.ToInt64(data, 0);
    }

    public static void WriteInt64(this Stream stream, long value, bool isBigEndian = false)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        if (isBigEndian)
        {
            Array.Reverse(bytes);
        }
        stream.Write(bytes, 0, bytes.Length);
    }

    public static ulong ReadUInt64(this Stream stream, bool isBigEndian = false)
    {
        stream.Read(data, 0, 8);
        if (isBigEndian)
        {
            Array.Reverse(data, 0, 8);
        }
        return BitConverter.ToUInt64(data, 0);
    }

    public static void WriteUInt64(this Stream stream, ulong value, bool isBigEndian = false)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        if (isBigEndian)
        {
            Array.Reverse(bytes);
        }
        stream.Write(bytes, 0, bytes.Length);
    }

    public static float ReadFloat(this Stream stream)
    {
        stream.Read(data, 0, 4);
        return BitConverter.ToSingle(data, 0);
    }

    public static void WriteFloat(this Stream stream, float value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        stream.Write(bytes, 0, bytes.Length);
    }




    public static byte[] ReadBytesCached(this Stream stream, int len)
    {
        if (bufferCache == null || bufferCache.Length < len)
        {
            bufferCache = new byte[len];
        }
        stream.Read(bufferCache, 0, len);
        return bufferCache;
    }

    public static void ReadAndDiscard(this Stream stream, int len)
    {
        while (len > discardBuffer.Length)
        {
            stream.Read(discardBuffer, 0, discardBuffer.Length);
            len -= discardBuffer.Length;
        }
        stream.Read(discardBuffer, 0, len);
    }


}



