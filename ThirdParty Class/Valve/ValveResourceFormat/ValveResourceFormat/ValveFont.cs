using System.IO;
using System.Text;

namespace ValveResourceFormat
{
	public class ValveFont
	{
		private const string MAGIC = "VFONT1";

		private const byte MAGIC_TRICK = 167;

		public byte[] Read(string filename)
		{
			using (FileStream input = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				return Read(input);
			}
		}

		public byte[] Read(Stream input)
		{
			using (BinaryReader binaryReader = new BinaryReader(input))
			{
				binaryReader.BaseStream.Seek(checked(-"VFONT1".Length), SeekOrigin.End);
				if (Encoding.ASCII.GetString(binaryReader.ReadBytes("VFONT1".Length)) != "VFONT1")
				{
					throw new InvalidDataException("Given file is not a vfont, version 1.");
				}
				return Decode(binaryReader);
			}
		}

		private byte[] Decode(BinaryReader reader)
		{
			checked
			{
				reader.BaseStream.Seek(-1 - "VFONT1".Length, SeekOrigin.End);
				byte b = reader.ReadByte();
				byte[] array = new byte[reader.BaseStream.Length - "VFONT1".Length - unchecked((long)b)];
				int num = 167;
				reader.BaseStream.Seek(-b, SeekOrigin.Current);
				b = (byte)(unchecked((uint)b) - 1u);
				for (int i = 0; i < b; i++)
				{
					unchecked
					{
						num ^= checked(unchecked((int)reader.ReadByte()) + 167) % 256;
					}
				}
				reader.BaseStream.Seek(0L, SeekOrigin.Begin);
				for (int j = 0; j < array.Length; j++)
				{
					byte b2 = reader.ReadByte();
					array[j] = (byte)(b2 ^ num);
					unchecked
					{
						num = checked(unchecked((int)b2) + 167) % 256;
					}
				}
				return array;
			}
		}
	}
}
