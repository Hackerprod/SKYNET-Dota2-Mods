using System;
using System.CodeDom.Compiler;
using System.IO;
using ValveResourceFormat.Blocks;

namespace ValveResourceFormat.ResourceTypes.NTROSerialization
{
	public abstract class NTROValue
	{
		public DataType Type
		{
			get;
			protected set;
		}

		public bool Pointer
		{
			get;
			protected set;
		}

		public abstract void WriteText(IndentedTextWriter writer);
	}
	public class NTROValue<T> : NTROValue
	{
		public T Value
		{
			get;
			private set;
		}

		public NTROValue(DataType type, T value, bool pointer = false)
		{
			base.Type = type;
			Value = value;
			base.Pointer = pointer;
		}

		public override string ToString()
		{
			using (StringWriter stringWriter = new StringWriter())
			{
				using (IndentedTextWriter writer = new IndentedTextWriter(stringWriter, "\t"))
				{
					WriteText(writer);
					return stringWriter.ToString();
				}
			}
		}

		public override void WriteText(IndentedTextWriter writer)
		{
			if (Value == null)
			{
				if (base.Type == DataType.ExternalReference)
				{
					writer.WriteLine("ID: {0:X16}", 0);
				}
				else
				{
					writer.WriteLine("NULL");
				}
			}
			else
			{
				T value;
				switch (base.Type)
				{
				case DataType.Enum:
					writer.WriteLine("0x{0:X8}", Value);
					break;
				case DataType.Byte:
					writer.WriteLine("0x{0:X2}", Value);
					break;
				case DataType.Boolean:
					value = Value;
					writer.WriteLine(value.ToString().ToLower());
					break;
				case DataType.UInt16:
					writer.WriteLine("0x{0:X4}", Value);
					break;
				case DataType.UInt32:
					writer.WriteLine("0x{0:X8}", Value);
					break;
				case DataType.Float:
					writer.WriteLine("{0:F6}", Value);
					break;
				case DataType.UInt64:
					writer.WriteLine("0x{0:X16}", Value);
					break;
				case DataType.ExternalReference:
				{
					ResourceExtRefList.ResourceReferenceInfo resourceReferenceInfo = Value as ResourceExtRefList.ResourceReferenceInfo;
					writer.WriteLine("ID: {0:X16}", resourceReferenceInfo.Id);
					break;
				}
				case DataType.Vector4D:
				case DataType.Quaternion:
				case DataType.Fltx4:
				case DataType.Color:
				case DataType.Vector4D_44:
				{
					Vector4 vector = Value as Vector4;
					if (base.Type == DataType.Quaternion)
					{
						writer.WriteLine("{{x: {0:F}, y: {1:F}, z: {2:F}, w: {3}}}", vector.X, vector.Y, vector.Z, vector.W.ToString("F"));
					}
					else
					{
						writer.WriteLine("({0:F6}, {1:F6}, {2:F6}, {3:F6})", vector.X, vector.Y, vector.Z, vector.W);
					}
					break;
				}
				case DataType.String4:
				case DataType.String:
					writer.WriteLine("\"{0}\"", Value);
					break;
				case DataType.SByte:
				case DataType.Int16:
				case DataType.Int32:
				case DataType.Int64:
				case DataType.Vector:
				case DataType.CTransform:
					writer.WriteLine(Value);
					break;
				case DataType.Matrix3x4:
				case DataType.Matrix3x4a:
					(Value as Matrix3x4).WriteText(writer);
					break;
				case DataType.Struct:
					(Value as NTROStruct).WriteText(writer);
					break;
				default:
					value = Value;
					throw new NotImplementedException($"Unknown data type: {value.GetType()}");
				}
			}
		}
	}
}
