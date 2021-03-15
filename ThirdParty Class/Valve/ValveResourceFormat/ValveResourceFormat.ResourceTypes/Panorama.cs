using System.Collections.Generic;
using System.IO;
using System.Text;
using ValveResourceFormat.Blocks;

namespace ValveResourceFormat.ResourceTypes
{
	public class Panorama : ResourceData
	{
		public class NameEntry
		{
			public string Name
			{
				get;
				set;
			}

			public uint Unknown1
			{
				get;
				set;
			}

			public uint Unknown2
			{
				get;
				set;
			}
		}

		public List<NameEntry> Names
		{
			get;
		}

		public byte[] Data
		{
			get;
			private set;
		}

		public uint CRC32
		{
			get;
			private set;
		}

		public Panorama()
		{
			Names = new List<NameEntry>();
		}

		public override void Read(BinaryReader reader, Resource resource)
		{
			reader.BaseStream.Position = base.Offset;
			CRC32 = reader.ReadUInt32();
			ushort num = reader.ReadUInt16();
			checked
			{
				for (int i = 0; i < num; i++)
				{
					NameEntry item = new NameEntry
					{
						Name = reader.ReadNullTermString(Encoding.UTF8),
						Unknown1 = reader.ReadUInt32(),
						Unknown2 = reader.ReadUInt32()
					};
					Names.Add(item);
				}
				long num2 = reader.BaseStream.Position - unchecked((long)base.Offset);
				Data = reader.ReadBytes((int)base.Size - (int)num2);
				if (Crc32.Compute(Data) != CRC32)
				{
					throw new InvalidDataException("CRC32 mismatch for read data.");
				}
			}
		}

		public override string ToString()
		{
			return Encoding.UTF8.GetString(Data);
		}
	}
}
