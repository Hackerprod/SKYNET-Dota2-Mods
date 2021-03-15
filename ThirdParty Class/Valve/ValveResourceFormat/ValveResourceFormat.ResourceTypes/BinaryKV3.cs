using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using ValveResourceFormat.Blocks;
using ValveResourceFormat.KeyValues;

namespace ValveResourceFormat.ResourceTypes
{
	public class BinaryKV3 : ResourceData
	{
		private static readonly byte[] ENCODING = new byte[16]
		{
			70,
			26,
			121,
			149,
			188,
			149,
			108,
			79,
			167,
			11,
			5,
			188,
			161,
			183,
			223,
			210
		};

		private static readonly byte[] FORMAT = new byte[16]
		{
			124,
			22,
			18,
			116,
			233,
			6,
			152,
			70,
			175,
			242,
			230,
			62,
			181,
			144,
			55,
			231
		};

		private static readonly byte[] SIG = new byte[4]
		{
			86,
			75,
			86,
			3
		};

		private string[] stringArray;

		public KVObject Data
		{
			get;
			private set;
		}

		public override void Read(BinaryReader reader, Resource resource)
		{
			reader.BaseStream.Position = base.Offset;
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			BinaryReader binaryReader = new BinaryReader(memoryStream);
			byte[] bytes = reader.ReadBytes(4);
			if (Encoding.ASCII.GetString(bytes) != Encoding.ASCII.GetString(SIG))
			{
				throw new InvalidDataException("Invalid KV Signature");
			}
			byte[] bytes2 = reader.ReadBytes(16);
			if (Encoding.ASCII.GetString(bytes2) != Encoding.ASCII.GetString(ENCODING))
			{
				throw new InvalidDataException("Unrecognized KV3 Encoding");
			}
			byte[] bytes3 = reader.ReadBytes(16);
			if (Encoding.ASCII.GetString(bytes3) != Encoding.ASCII.GetString(FORMAT))
			{
				throw new InvalidDataException("Unrecognised KV3 Format");
			}
			byte[] array = reader.ReadBytes(4);
			checked
			{
				if ((array[3] & 0x80) > 0)
				{
					binaryWriter.Write(reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position)));
				}
				else
				{
					bool flag = true;
					while (reader.BaseStream.Position != reader.BaseStream.Length && flag)
					{
						try
						{
							ushort num = reader.ReadUInt16();
							for (int i = 0; i < 16; i++)
							{
								if ((num & (1 << i)) > 0)
								{
									ushort num2 = reader.ReadUInt16();
									int num3 = ((num2 & 0xFFF0) >> 4) + 1;
									int num4 = (num2 & 0xF) + 3;
									int num5 = (num3 < num4) ? num3 : num4;
									long position = binaryReader.BaseStream.Position;
									binaryReader.BaseStream.Position = position - num3;
									byte[] buffer = binaryReader.ReadBytes(num5);
									binaryWriter.BaseStream.Position = position;
									while (num4 > 0)
									{
										binaryWriter.Write(buffer, 0, (num5 < num4) ? num5 : num4);
										num4 -= num5;
									}
								}
								else
								{
									byte value = reader.ReadByte();
									binaryWriter.Write(value);
								}
								if (binaryWriter.BaseStream.Length == (array[2] << 16) + (array[1] << 8) + unchecked((int)array[0]))
								{
									flag = false;
									break;
								}
							}
						}
						catch (EndOfStreamException)
						{
							goto IL_0224;
						}
					}
				}
				goto IL_0224;
			}
			IL_0224:
			binaryReader.BaseStream.Position = 0L;
			uint num6 = binaryReader.ReadUInt32();
			stringArray = new string[num6];
			for (int j = 0; j < num6; j = checked(j + 1))
			{
				stringArray[j] = binaryReader.ReadNullTermString(Encoding.UTF8);
			}
			Data = ParseBinaryKV3(binaryReader, null, inArray: true);
		}

		private KVObject ParseBinaryKV3(BinaryReader reader, KVObject parent, bool inArray = false)
		{
			string name = null;
			if (!inArray)
			{
				int num = reader.ReadInt32();
				name = ((num == -1) ? string.Empty : stringArray[num]);
			}
			byte b = reader.ReadByte();
			KVFlag flag = KVFlag.None;
			if ((b & 0x80) > 0)
			{
				b = (byte)(b & 0x7F);
				flag = (KVFlag)reader.ReadByte();
			}
			checked
			{
				switch (b)
				{
				case 1:
					parent.AddProperty(name, MakeValue(b, null, flag));
					break;
				case 2:
					parent.AddProperty(name, MakeValue(b, reader.ReadBoolean(), flag));
					break;
				case 3:
					parent.AddProperty(name, MakeValue(b, reader.ReadInt64(), flag));
					break;
				case 5:
					parent.AddProperty(name, MakeValue(b, reader.ReadDouble(), flag));
					break;
				case 6:
				{
					int num3 = reader.ReadInt32();
					parent.AddProperty(name, MakeValue(b, (num3 == -1) ? string.Empty : stringArray[num3], flag));
					break;
				}
				case 8:
				{
					int num4 = reader.ReadInt32();
					KVObject kVObject2 = new KVObject(name, isArray: true);
					for (int j = 0; j < num4; j++)
					{
						ParseBinaryKV3(reader, kVObject2, inArray: true);
					}
					parent.AddProperty(name, MakeValue(b, kVObject2, flag));
					break;
				}
				case 9:
				{
					int num2 = reader.ReadInt32();
					KVObject kVObject = new KVObject(name, isArray: false);
					for (int i = 0; i < num2; i++)
					{
						ParseBinaryKV3(reader, kVObject);
					}
					if (parent == null)
					{
						parent = kVObject;
					}
					else
					{
						parent.AddProperty(name, MakeValue(b, kVObject, flag));
					}
					break;
				}
				default:
					throw new InvalidDataException($"Unknown KVType {b}");
				}
				return parent;
			}
		}

		private KVValue MakeValue(byte type, object data, KVFlag flag)
		{
			if (flag != 0)
			{
				return new KVFlaggedValue((KVType)type, flag, data);
			}
			return new KVValue((KVType)type, data);
		}

		public override void WriteText(IndentedTextWriter writer)
		{
			Data.Serialize(writer);
		}
	}
}
