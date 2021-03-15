using System;
using System.IO;
using System.Text;

public class Bitstream : Stream
{
    private const int COORD_INTEGER_BITS = 14;

    private const int COORD_FRACTIONAL_BITS = 5;

    private const int COORD_DENOMINATOR = 32;

    private const double COORD_RESOLUTION = 0.03125;

    private uint[] data;

    public override long Position
    {
        get;
        set;
    }

    public long BitLength
    {
        get;
        private set;
    }

    public override long Length => (BitLength + 7L) / 8L;

    public bool Eof => BitLength == Position;

    public long Remain => BitLength - Position;

    public override bool CanRead => true;

    public override bool CanSeek => true;

    public override bool CanWrite => true;

    private Bitstream(uint[] data, uint byteLength)
    {
        this.data = data;
        Position = 0L;
        BitLength = 8 * byteLength;
    }

    public static Bitstream Create()
    {
        return new Bitstream(new uint[32], 0u);
    }

    public static Bitstream CreateWith(byte[] bytes)
    {
        return CreateWith(bytes, bytes.Length);
    }

    public static Bitstream CreateWith(byte[] bytes, int length)
    {
        byte[] array = new byte[length + 3];
        Array.Copy(bytes, array, length);
        uint[] array2 = new uint[array.Length / 4];
        for (int i = 0; i < array2.Length; i++)
        {
            uint num = array2[i] = BitConverter.ToUInt32(array, i * 4);
        }
        return new Bitstream(array2, (uint)length);
    }

    public void Erase()
    {
        Position = 0L;
        BitLength = 0L;
    }

    public uint ReadBits(byte n)
    {
        if (n > 32)
        {
            throw new ArgumentException("Can't grab more than 32 bits");
        }
        if (!HasBits(n))
        {
            throw new ArgumentException("Not enough bits remain in the buffer");
        }
        uint num = data[Position / 32L];
        uint num2 = data[(Position + n - 1L) / 32L];
        uint num3 = (uint)((int)Position & 0x1F);
        uint num4 = num >> (int)(byte)num3;
        num2 <<= (int)(byte)(32 - num3);
        uint num5 = (uint)((1L << (int)n) - 1L);
        uint result = (num4 | num2) & num5;
        Position += n;
        return result;
    }

    public bool HasBits(long n)
    {
        return n <= Remain;
    }

    public bool HasByte()
    {
        return HasBits(8L);
    }

    public bool ReadBool()
    {
        return ReadBits(1) == 1;
    }

    public byte[] ReadManyBits(uint bits)
    {
        byte[] array = new byte[(bits + 7) / 8u];
        for (uint num = 0u; num < bits / 8u; num++)
        {
            array[num] = ReadByte();
        }
        if (bits % 8u != 0)
        {
            byte n = (byte)(bits % 8u);
            array[array.Length - 1] = (byte)ReadBits(n);
        }
        return array;
    }

    public byte ReadByte(bool pad = false)
    {
        if (!pad)
        {
            return (byte)ReadBits(8);
        }
        byte b = (byte)Math.Min(8L, Remain);
        return (byte)((-1 << (int)b) | (int)ReadBits(b));
    }

    public char ReadChar()
    {
        return (char)ReadBits(8);
    }

    public string ReadString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        for (char c = ReadChar(); c != 0; c = ReadChar())
        {
            stringBuilder.Append(c);
        }
        return stringBuilder.ToString();
    }

    public ushort ReadUInt16()
    {
        return (ushort)ReadBits(16);
    }

    public uint ReadUInt32()
    {
        return ReadBits(32);
    }

    public ulong ReadUInt64()
    {
        ulong num = ReadBits(32);
        return ((ulong)ReadBits(32) << 32) | num;
    }

    public uint ReadVarUInt()
    {
        int num = 0;
        uint num2 = 0u;
        uint num3;
        do
        {
            num3 = ReadBits(8);
            uint num4 = num3 & 0x7F;
            num2 |= num4 << num;
            num += 7;
        }
        while (num3 >> 7 != 0 && num < 35);
        return num2;
    }

    public byte[] ToBytes()
    {
        byte[] array = new byte[Length];
        long position = Position;
        Position = 0L;
        Read(array, 0, array.Length);
        Position = position;
        return array;
    }

    public void WriteBitCoord(float f)
    {
        bool b = (double)f <= -0.03125;
        uint num = (uint)Math.Abs(f);
        uint num2 = (uint)(Math.Abs((int)(f * 32f)) & 0x1F);
        WriteBool(num != 0);
        WriteBool(num2 != 0);
        if (num != 0 || num2 != 0)
        {
            WriteBool(b);
            if (num != 0)
            {
                WriteBits(num - 1, 14);
            }
            if (num2 != 0)
            {
                WriteBits(num2, 5);
            }
        }
    }

    public void WriteBitVec3Coord(Vector v)
    {
        bool flag = (double)v.X <= -0.03125 || 0.03125 <= (double)v.X;
        bool flag2 = (double)v.Y <= -0.03125 || 0.03125 <= (double)v.Y;
        bool flag3 = (double)v.Z <= -0.03125 || 0.03125 <= (double)v.Z;
        WriteBool(flag);
        WriteBool(flag2);
        WriteBool(flag3);
        if (flag)
        {
            WriteBitCoord(v.X);
        }
        if (flag2)
        {
            WriteBitCoord(v.Y);
        }
        if (flag3)
        {
            WriteBitCoord(v.Z);
        }
    }

    public void WriteBits(uint value, byte n)
    {
        if (n > 32)
        {
            throw new ArgumentException("Can't write more than 32 bits");
        }
        if (value != (value & ((1L << (int)n) - 1L)))
        {
            throw new ArgumentException("Value cannot fit in requested bits");
        }
        Allocate(n);
        byte b = (byte)(Position & 0x1F);
        if (b + n <= 32)
        {
            long num = Position / 32L;
            uint num2 = data[num];
            uint num3 = (uint)(4294967295L << b + n);
            uint num4 = (uint)((1L << (int)b) - 1L);
            data[num] = ((num2 & num3) | (value << (int)b) | (num2 & num4));
        }
        else
        {
            long num5 = Position / 32L;
            uint num6 = data[num5];
            uint num7 = (uint)((1L << (int)b) - 1L);
            data[num5] = ((value << (int)b) | (num6 & num7));
            byte b2 = (byte)((b + n) % 32);
            uint num8 = data[num5 + 1L];
            uint num9 = (uint)(4294967295L << (int)b2);
            data[num5 + 1L] = ((num9 & num8) | (value >> n - b2));
        }
        if (Position == BitLength)
        {
            BitLength += n;
        }
        Position += n;
    }

    public void WriteBool(bool b)
    {
        WriteBits((uint)(b ? 1 : 0), 1);
    }

    public override void WriteByte(byte b)
    {
        WriteBits(b, 8);
    }

    public void WriteChar(char c)
    {
        WriteBits(c, 8);
    }

    public void WriteFloat(float f)
    {
        byte[] bytes = BitConverter.GetBytes(f);
        Write(bytes);
    }

    public void WriteInt16(short value)
    {
        if (value < 0)
        {
            WriteBits((ushort)(-value), 15);
            WriteBool(b: true);
        }
        else
        {
            WriteBits((ushort)value, 15);
            WriteBool(b: false);
        }
    }

    public void WriteUInt16(ushort value)
    {
        WriteBits(value, 16);
    }

    public void WriteUInt32(uint value)
    {
        WriteBits(value, 32);
    }

    public void WriteVarUInt(uint value)
    {
        do
        {
            uint num = value & 0x7F;
            value >>= 7;
            uint num2 = (uint)((value != 0) ? 128 : 0);
            WriteBits(num2 | num, 8);
        }
        while (value != 0);
    }

    private void Allocate(long bits)
    {
        long num = (Position + bits + 31L) / 32L;
        BitLength = Math.Max(Position + bits, BitLength);
        if (num > data.Length)
        {
            long num2;
            for (num2 = data.Length; num2 < num; num2 *= 2L)
            {
            }
            uint[] destinationArray = new uint[num2];
            Array.Copy(data, destinationArray, data.Length);
            data = destinationArray;
        }
    }

    public override void Flush()
    {
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (offset + count > buffer.Length)
        {
            throw new ArgumentException("offset + count > buffer.length");
        }
        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException("offset is negative");
        }
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException("count is negative");
        }
        int num = 0;
        while (HasBits(8L) && num < count)
        {
            buffer[num + offset] = ReadByte();
            num++;
        }
        if (HasBits(1L) && num < count)
        {
            byte b = (byte)Remain;
            buffer[num] = (byte)((-1 << (int)b) | (int)ReadBits(b));
        }
        return num;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        switch (origin)
        {
            case SeekOrigin.Begin:
                Position = offset * 8L;
                break;
            case SeekOrigin.Current:
                Position += offset * 8L;
                break;
            case SeekOrigin.End:
                Position = BitLength - offset * 8L;
                break;
            default:
                throw new ArgumentException("Unsupported SeekOrigin " + origin);
        }
        return Position;
    }

    public override void SetLength(long newLength)
    {
        if (newLength < Length)
        {
            throw new InvalidOperationException("Can't shrink a bitstream");
        }
        long position = Position;
        Position = 0L;
        Allocate(newLength * 8L);
        Position = position;
    }

    public void Write(byte[] buffer)
    {
        Write(buffer, 0, buffer.Length);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        Allocate(count * 8);
        for (int i = offset; i < offset + count; i++)
        {
            WriteBits(buffer[i], 8);
        }
    }
}
