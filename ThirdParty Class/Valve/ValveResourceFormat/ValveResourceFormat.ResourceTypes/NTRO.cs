using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ValveResourceFormat.Blocks;
using ValveResourceFormat.ResourceTypes.NTROSerialization;

namespace ValveResourceFormat.ResourceTypes
{
	public class NTRO : ResourceData
	{
		protected BinaryReader Reader
		{
			get;
			private set;
		}

		protected Resource Resource
		{
			get;
			private set;
		}

		public NTROStruct Output
		{
			get;
			private set;
		}

		public override void Read(BinaryReader reader, Resource resource)
		{
			Reader = reader;
			Resource = resource;
			using (List<ResourceIntrospectionManifest.ResourceDiskStruct>.Enumerator enumerator = resource.IntrospectionManifest.ReferencedStructs.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					ResourceIntrospectionManifest.ResourceDiskStruct current = enumerator.Current;
					Output = ReadStructure(current, base.Offset);
				}
			}
		}

		private NTROStruct ReadStructure(ResourceIntrospectionManifest.ResourceDiskStruct refStruct, long startingOffset)
		{
			NTROStruct structEntry = new NTROStruct(refStruct.Name);
			checked
			{
				foreach (ResourceIntrospectionManifest.ResourceDiskStruct.Field item in refStruct.FieldIntrospection)
				{
					Reader.BaseStream.Position = startingOffset + item.OnDiskOffset;
					ReadFieldIntrospection(item, ref structEntry);
				}
				Reader.BaseStream.Position = startingOffset + unchecked((long)refStruct.DiskSize);
				if (refStruct.BaseStructId != 0)
				{
					long position = Reader.BaseStream.Position;
					ResourceIntrospectionManifest.ResourceDiskStruct resourceDiskStruct = Resource.IntrospectionManifest.ReferencedStructs.First((ResourceIntrospectionManifest.ResourceDiskStruct x) => x.Id == refStruct.BaseStructId);
					foreach (ResourceIntrospectionManifest.ResourceDiskStruct.Field item2 in resourceDiskStruct.FieldIntrospection)
					{
						Reader.BaseStream.Position = startingOffset + item2.OnDiskOffset;
						ReadFieldIntrospection(item2, ref structEntry);
					}
					Reader.BaseStream.Position = position;
				}
				return structEntry;
			}
		}

		private void ReadFieldIntrospection(ResourceIntrospectionManifest.ResourceDiskStruct.Field field, ref NTROStruct structEntry)
		{
			checked
			{
				uint num = (uint)field.Count;
				bool pointer = false;
				if (num == 0)
				{
					num = 1u;
				}
				long num2 = 0L;
				if (field.Indirections.Count > 0)
				{
					if (field.Indirections.Count > 1)
					{
						throw new NotImplementedException("More than one indirection, not yet handled.");
					}
					if (field.Count > 0)
					{
						throw new NotImplementedException("Indirection.Count > 0 && field.Count > 0");
					}
					byte b = field.Indirections[0];
					uint num3 = Reader.ReadUInt32();
					switch (b)
					{
					case 3:
						pointer = true;
						if (num3 == 0)
						{
							structEntry.Add(field.FieldName, new NTROValue<byte?>(field.Type, null, pointer: true));
							return;
						}
						num2 = Reader.BaseStream.Position;
						Reader.BaseStream.Position += num3 - 4u;
						break;
					case 4:
						num = Reader.ReadUInt32();
						num2 = Reader.BaseStream.Position;
						if (num != 0)
						{
							Reader.BaseStream.Position += num3 - 8u;
						}
						break;
					default:
						throw new NotImplementedException($"Unknown indirection. ({b})");
					}
				}
				if (field.Count > 0 || field.Indirections.Count > 0)
				{
					NTROArray nTROArray = new NTROArray(field.Type, (int)num, pointer, field.Indirections.Count > 0);
					for (int i = 0; i < num; i++)
					{
						nTROArray[i] = ReadField(field, pointer);
					}
					structEntry.Add(field.FieldName, nTROArray);
				}
				else
				{
					for (int j = 0; j < num; j++)
					{
						structEntry.Add(field.FieldName, ReadField(field, pointer));
					}
				}
				if (num2 > 0)
				{
					Reader.BaseStream.Position = num2;
				}
			}
		}

		private NTROValue ReadField(ResourceIntrospectionManifest.ResourceDiskStruct.Field field, bool pointer)
		{
			switch (field.Type)
			{
			case DataType.Struct:
			{
				ResourceIntrospectionManifest.ResourceDiskStruct refStruct = Resource.IntrospectionManifest.ReferencedStructs.First((ResourceIntrospectionManifest.ResourceDiskStruct x) => x.Id == field.TypeData);
				return new NTROValue<NTROStruct>(field.Type, ReadStructure(refStruct, Reader.BaseStream.Position), pointer);
			}
			case DataType.Enum:
				return new NTROValue<uint>(field.Type, Reader.ReadUInt32(), pointer);
			case DataType.SByte:
				return new NTROValue<sbyte>(field.Type, Reader.ReadSByte(), pointer);
			case DataType.Byte:
				return new NTROValue<byte>(field.Type, Reader.ReadByte(), pointer);
			case DataType.Boolean:
				return new NTROValue<bool>(field.Type, (Reader.ReadByte() == 1) ? true : false, pointer);
			case DataType.Int16:
				return new NTROValue<short>(field.Type, Reader.ReadInt16(), pointer);
			case DataType.UInt16:
				return new NTROValue<ushort>(field.Type, Reader.ReadUInt16(), pointer);
			case DataType.Int32:
				return new NTROValue<int>(field.Type, Reader.ReadInt32(), pointer);
			case DataType.UInt32:
				return new NTROValue<uint>(field.Type, Reader.ReadUInt32(), pointer);
			case DataType.Float:
				return new NTROValue<float>(field.Type, Reader.ReadSingle(), pointer);
			case DataType.Int64:
				return new NTROValue<long>(field.Type, Reader.ReadInt64(), pointer);
			case DataType.ExternalReference:
			{
				ulong id = Reader.ReadUInt64();
				ResourceExtRefList.ResourceReferenceInfo value5 = (id == 0) ? null : Resource.ExternalReferences?.ResourceRefInfoList.FirstOrDefault((ResourceExtRefList.ResourceReferenceInfo c) => c.Id == id);
				return new NTROValue<ResourceExtRefList.ResourceReferenceInfo>(field.Type, value5, pointer);
			}
			case DataType.UInt64:
				return new NTROValue<ulong>(field.Type, Reader.ReadUInt64(), pointer);
			case DataType.Vector:
			{
				Vector3 value4 = new Vector3(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle());
				return new NTROValue<Vector3>(field.Type, value4, pointer);
			}
			case DataType.Vector4D:
			case DataType.Quaternion:
			case DataType.Fltx4:
			case DataType.Color:
			case DataType.Vector4D_44:
			{
				Vector4 value3 = new Vector4(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle());
				return new NTROValue<Vector4>(field.Type, value3, pointer);
			}
			case DataType.String4:
			case DataType.String:
				return new NTROValue<string>(field.Type, Reader.ReadOffsetString(Encoding.UTF8), pointer);
			case DataType.Matrix3x4:
			case DataType.Matrix3x4a:
			{
				Matrix3x4 value2 = new Matrix3x4(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle());
				return new NTROValue<Matrix3x4>(field.Type, value2, pointer);
			}
			case DataType.CTransform:
			{
				CTransform value = new CTransform(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle());
				return new NTROValue<CTransform>(field.Type, value, pointer);
			}
			default:
				throw new NotImplementedException($"Unknown data type: {field.Type} (name: {field.FieldName})");
			}
		}

		public override string ToString()
		{
			return Output.ToString() ?? "Nope.";
		}
	}
}
