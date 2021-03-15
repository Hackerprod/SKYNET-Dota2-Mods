using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ValveResourceFormat.Blocks
{
	public class ResourceIntrospectionManifest : Block
	{
		public class ResourceDiskStruct
		{
			public class Field
			{
				public string FieldName
				{
					get;
					set;
				}

				public short Count
				{
					get;
					set;
				}

				public short OnDiskOffset
				{
					get;
					set;
				}

				public List<byte> Indirections
				{
					get;
					private set;
				}

				public uint TypeData
				{
					get;
					set;
				}

				public DataType Type
				{
					get;
					set;
				}

				public Field()
				{
					Indirections = new List<byte>();
				}

				public void WriteText(IndentedTextWriter writer)
				{
					writer.WriteLine("CResourceDiskStructField");
					writer.WriteLine("{");
					checked
					{
						writer.Indent++;
						writer.WriteLine("CResourceString m_pFieldName = \"{0}\"", FieldName);
						writer.WriteLine("int16 m_nCount = {0}", Count);
						writer.WriteLine("int16 m_nOnDiskOffset = {0}", OnDiskOffset);
						writer.WriteLine("uint8[{0}] m_Indirection =", Indirections.Count);
						writer.WriteLine("[");
						writer.Indent++;
						foreach (byte indirection in Indirections)
						{
							writer.WriteLine("{0:D2}", indirection);
						}
						writer.Indent--;
						writer.WriteLine("]");
						writer.WriteLine("uint32 m_nTypeData = 0x{0:X8}", TypeData);
						writer.WriteLine("int16 m_nType = {0}", unchecked((int)Type));
						writer.Indent--;
						writer.WriteLine("}");
					}
				}
			}

			public uint IntrospectionVersion
			{
				get;
				set;
			}

			public uint Id
			{
				get;
				set;
			}

			public string Name
			{
				get;
				set;
			}

			public uint DiskCrc
			{
				get;
				set;
			}

			public int UserVersion
			{
				get;
				set;
			}

			public ushort DiskSize
			{
				get;
				set;
			}

			public ushort Alignment
			{
				get;
				set;
			}

			public uint BaseStructId
			{
				get;
				set;
			}

			public byte StructFlags
			{
				get;
				set;
			}

			public List<Field> FieldIntrospection
			{
				get;
				private set;
			}

			public ResourceDiskStruct()
			{
				FieldIntrospection = new List<Field>();
			}

			public void WriteText(IndentedTextWriter writer)
			{
				writer.WriteLine("CResourceDiskStruct");
				writer.WriteLine("{");
				checked
				{
					writer.Indent++;
					writer.WriteLine("uint32 m_nIntrospectionVersion = 0x{0:X8}", IntrospectionVersion);
					writer.WriteLine("uint32 m_nId = 0x{0:X8}", Id);
					writer.WriteLine("CResourceString m_pName = \"{0}\"", Name);
					writer.WriteLine("uint32 m_nDiskCrc = 0x{0:X8}", DiskCrc);
					writer.WriteLine("int32 m_nUserVersion = {0}", UserVersion);
					writer.WriteLine("uint16 m_nDiskSize = 0x{0:X4}", DiskSize);
					writer.WriteLine("uint16 m_nAlignment = 0x{0:X4}", Alignment);
					writer.WriteLine("uint32 m_nBaseStructId = 0x{0:X8}", BaseStructId);
					writer.WriteLine("Struct m_FieldIntrospection[{0}] =", FieldIntrospection.Count);
					writer.WriteLine("[");
					writer.Indent++;
					foreach (Field item in FieldIntrospection)
					{
						item.WriteText(writer);
					}
					writer.Indent--;
					writer.WriteLine("]");
					writer.WriteLine("uint8 m_nStructFlags = 0x{0:X2}", StructFlags);
					writer.Indent--;
					writer.WriteLine("}");
				}
			}
		}

		public class ResourceDiskEnum
		{
			public class Value
			{
				public string EnumValueName
				{
					get;
					set;
				}

				public int EnumValue
				{
					get;
					set;
				}

				public void WriteText(IndentedTextWriter writer)
				{
					writer.WriteLine("CResourceDiskEnumValue");
					writer.WriteLine("{");
					checked
					{
						writer.Indent++;
						writer.WriteLine("CResourceString m_pEnumValueName = \"{0}\"", EnumValueName);
						writer.WriteLine("int32 m_nEnumValue = {0}", EnumValue);
						writer.Indent--;
						writer.WriteLine("}");
					}
				}
			}

			public uint IntrospectionVersion
			{
				get;
				set;
			}

			public uint Id
			{
				get;
				set;
			}

			public string Name
			{
				get;
				set;
			}

			public uint DiskCrc
			{
				get;
				set;
			}

			public int UserVersion
			{
				get;
				set;
			}

			public List<Value> EnumValueIntrospection
			{
				get;
				private set;
			}

			public ResourceDiskEnum()
			{
				EnumValueIntrospection = new List<Value>();
			}

			public void WriteText(IndentedTextWriter writer)
			{
				writer.WriteLine("CResourceDiskEnum");
				writer.WriteLine("{");
				checked
				{
					writer.Indent++;
					writer.WriteLine("uint32 m_nIntrospectionVersion = 0x{0:X8}", IntrospectionVersion);
					writer.WriteLine("uint32 m_nId = 0x{0:X8}", Id);
					writer.WriteLine("CResourceString m_pName = \"{0}\"", Name);
					writer.WriteLine("uint32 m_nDiskCrc = 0x{0:X8}", DiskCrc);
					writer.WriteLine("int32 m_nUserVersion = {0}", UserVersion);
					writer.WriteLine("Struct m_EnumValueIntrospection[{0}] =", EnumValueIntrospection.Count);
					writer.WriteLine("[");
					writer.Indent++;
					foreach (Value item in EnumValueIntrospection)
					{
						item.WriteText(writer);
					}
					writer.Indent--;
					writer.WriteLine("]");
					writer.Indent--;
					writer.WriteLine("}");
				}
			}
		}

		public uint IntrospectionVersion
		{
			get;
			private set;
		}

		public List<ResourceDiskStruct> ReferencedStructs
		{
			get;
		}

		public List<ResourceDiskEnum> ReferencedEnums
		{
			get;
		}

		public ResourceIntrospectionManifest()
		{
			ReferencedStructs = new List<ResourceDiskStruct>();
			ReferencedEnums = new List<ResourceDiskEnum>();
		}

		public override BlockType GetChar()
		{
			return BlockType.NTRO;
		}

		public override void Read(BinaryReader reader, Resource resource)
		{
			reader.BaseStream.Position = base.Offset;
			IntrospectionVersion = reader.ReadUInt32();
			ReadStructs(reader);
			reader.BaseStream.Position = checked(base.Offset + 12u);
			ReadEnums(reader);
		}

		private void ReadStructs(BinaryReader reader)
		{
			uint num = reader.ReadUInt32();
			uint num2 = reader.ReadUInt32();
			checked
			{
				if (num2 != 0)
				{
					reader.BaseStream.Position += num - 8u;
					for (int i = 0; i < num2; i++)
					{
						ResourceDiskStruct resourceDiskStruct = new ResourceDiskStruct();
						resourceDiskStruct.IntrospectionVersion = reader.ReadUInt32();
						resourceDiskStruct.Id = reader.ReadUInt32();
						resourceDiskStruct.Name = reader.ReadOffsetString(Encoding.UTF8);
						resourceDiskStruct.DiskCrc = reader.ReadUInt32();
						resourceDiskStruct.UserVersion = reader.ReadInt32();
						resourceDiskStruct.DiskSize = reader.ReadUInt16();
						resourceDiskStruct.Alignment = reader.ReadUInt16();
						resourceDiskStruct.BaseStructId = reader.ReadUInt32();
						uint num3 = reader.ReadUInt32();
						uint num4 = reader.ReadUInt32();
						if (num4 != 0)
						{
							long position = reader.BaseStream.Position;
							reader.BaseStream.Position += num3 - 8u;
							for (int j = 0; j < num4; j++)
							{
								ResourceDiskStruct.Field field = new ResourceDiskStruct.Field();
								field.FieldName = reader.ReadOffsetString(Encoding.UTF8);
								field.Count = reader.ReadInt16();
								field.OnDiskOffset = reader.ReadInt16();
								uint num5 = reader.ReadUInt32();
								uint num6 = reader.ReadUInt32();
								if (num6 != 0)
								{
									long position2 = reader.BaseStream.Position;
									reader.BaseStream.Position += num5 - 8u;
									for (int k = 0; k < num6; k++)
									{
										field.Indirections.Add(reader.ReadByte());
									}
									reader.BaseStream.Position = position2;
								}
								field.TypeData = reader.ReadUInt32();
								field.Type = unchecked((DataType)reader.ReadInt16());
								reader.ReadBytes(2);
								resourceDiskStruct.FieldIntrospection.Add(field);
							}
							reader.BaseStream.Position = position;
						}
						resourceDiskStruct.StructFlags = reader.ReadByte();
						reader.ReadBytes(3);
						ReferencedStructs.Add(resourceDiskStruct);
					}
				}
			}
		}

		private void ReadEnums(BinaryReader reader)
		{
			uint num = reader.ReadUInt32();
			uint num2 = reader.ReadUInt32();
			checked
			{
				if (num2 != 0)
				{
					reader.BaseStream.Position += num - 8u;
					for (int i = 0; i < num2; i++)
					{
						ResourceDiskEnum resourceDiskEnum = new ResourceDiskEnum();
						resourceDiskEnum.IntrospectionVersion = reader.ReadUInt32();
						resourceDiskEnum.Id = reader.ReadUInt32();
						resourceDiskEnum.Name = reader.ReadOffsetString(Encoding.UTF8);
						resourceDiskEnum.DiskCrc = reader.ReadUInt32();
						resourceDiskEnum.UserVersion = reader.ReadInt32();
						uint num3 = reader.ReadUInt32();
						uint num4 = reader.ReadUInt32();
						if (num4 != 0)
						{
							long position = reader.BaseStream.Position;
							reader.BaseStream.Position += num3 - 8u;
							for (int j = 0; j < num4; j++)
							{
								ResourceDiskEnum.Value value = new ResourceDiskEnum.Value();
								value.EnumValueName = reader.ReadOffsetString(Encoding.UTF8);
								value.EnumValue = reader.ReadInt32();
								resourceDiskEnum.EnumValueIntrospection.Add(value);
							}
							reader.BaseStream.Position = position;
						}
						ReferencedEnums.Add(resourceDiskEnum);
					}
				}
			}
		}

		public override void WriteText(IndentedTextWriter writer)
		{
			writer.WriteLine("CResourceIntrospectionManifest");
			writer.WriteLine("{");
			checked
			{
				writer.Indent++;
				writer.WriteLine("uint32 m_nIntrospectionVersion = 0x{0:x8}", IntrospectionVersion);
				writer.WriteLine("Struct m_ReferencedStructs[{0}] =", ReferencedStructs.Count);
				writer.WriteLine("[");
				writer.Indent++;
				foreach (ResourceDiskStruct referencedStruct in ReferencedStructs)
				{
					referencedStruct.WriteText(writer);
				}
				writer.Indent--;
				writer.WriteLine("]");
				writer.WriteLine("Struct m_ReferencedEnums[{0}] =", ReferencedEnums.Count);
				writer.WriteLine("[");
				writer.Indent++;
				foreach (ResourceDiskEnum referencedEnum in ReferencedEnums)
				{
					referencedEnum.WriteText(writer);
				}
				writer.Indent--;
				writer.WriteLine("]");
				writer.Indent--;
				writer.WriteLine("}");
			}
		}
	}
}
