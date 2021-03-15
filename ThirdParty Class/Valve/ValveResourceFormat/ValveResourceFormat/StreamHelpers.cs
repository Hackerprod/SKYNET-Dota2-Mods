using System.IO;
using System.Text;

namespace ValveResourceFormat
{
	internal static class StreamHelpers
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

		public static string ReadOffsetString(this BinaryReader stream, Encoding encoding)
		{
			long position = stream.BaseStream.Position;
			uint num = stream.ReadUInt32();
			if (num == 0)
			{
				return string.Empty;
			}
			checked
			{
				stream.BaseStream.Position = position + unchecked((long)num);
				string result = stream.ReadNullTermString(encoding);
				stream.BaseStream.Position = position + 4;
				return result;
			}
		}
	}
}
