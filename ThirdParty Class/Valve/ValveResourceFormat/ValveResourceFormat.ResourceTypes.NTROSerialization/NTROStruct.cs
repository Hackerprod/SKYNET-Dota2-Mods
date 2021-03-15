using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace ValveResourceFormat.ResourceTypes.NTROSerialization
{
	public class NTROStruct : IDictionary, ICollection, IEnumerable
	{
		private readonly Dictionary<string, NTROValue> Contents;

		public string Name
		{
			get;
			private set;
		}

		public NTROValue this[string key]
		{
			get
			{
				return Contents[key];
			}
			set
			{
				Contents[key] = value;
			}
		}

		public object this[object key]
		{
			get
			{
				return ((IDictionary)Contents)[key];
			}
			set
			{
				((IDictionary)Contents)[key] = value;
			}
		}

		public int Count => Contents.Count;

		public bool IsFixedSize => ((IDictionary)Contents).IsFixedSize;

		public bool IsReadOnly => ((IDictionary)Contents).IsReadOnly;

		public bool IsSynchronized => ((ICollection)Contents).IsSynchronized;

		public ICollection Keys => Contents.Keys;

		public object SyncRoot => ((ICollection)Contents).SyncRoot;

		public ICollection Values => Contents.Values;

		public NTROStruct(string name)
		{
			Name = name;
			Contents = new Dictionary<string, NTROValue>();
		}

		public void WriteText(IndentedTextWriter writer)
		{
			writer.WriteLine(Name);
			writer.WriteLine("{");
			checked
			{
				writer.Indent++;
				foreach (KeyValuePair<string, NTROValue> content in Contents)
				{
					NTROArray nTROArray = content.Value as NTROArray;
					if (content.Value.Pointer)
					{
						writer.Write("{0} {1}* = (ptr) ->", ValveDataType(content.Value.Type), content.Key);
						content.Value.WriteText(writer);
					}
					else if (nTROArray != null)
					{
						if (nTROArray.Type == DataType.Byte && nTROArray.IsIndirection)
						{
							writer.WriteLine("{0}[{2}] {1} =", ValveDataType(nTROArray.Type), content.Key, nTROArray.Count);
						}
						else
						{
							writer.WriteLine("{0} {1}[{2}] =", ValveDataType(nTROArray.Type), content.Key, nTROArray.Count);
						}
						writer.WriteLine("[");
						writer.Indent++;
						foreach (NTROValue item in nTROArray)
						{
							if (nTROArray.Type == DataType.Byte && nTROArray.IsIndirection)
							{
								writer.WriteLine("{0:X2}", (item as NTROValue<byte>).Value);
							}
							else
							{
								item.WriteText(writer);
							}
						}
						writer.Indent--;
						writer.WriteLine("]");
					}
					else
					{
						writer.Write("{0} {1} = ", ValveDataType(content.Value.Type), content.Key);
						content.Value.WriteText(writer);
					}
				}
				writer.Indent--;
				writer.WriteLine("}");
			}
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

		private static string ValveDataType(DataType type)
		{
			switch (type)
			{
			case DataType.SByte:
				return "int8";
			case DataType.Byte:
				return "uint8";
			case DataType.Int16:
				return "int16";
			case DataType.UInt16:
				return "uint16";
			case DataType.Int32:
				return "int32";
			case DataType.UInt32:
				return "uint32";
			case DataType.Int64:
				return "int64";
			case DataType.UInt64:
				return "uint64";
			case DataType.Float:
				return "float32";
			case DataType.String:
				return "CResourceString";
			case DataType.Boolean:
				return "bool";
			case DataType.Fltx4:
				return "fltx4";
			case DataType.Matrix3x4a:
				return "matrix3x4a_t";
			default:
				return type.ToString();
			}
		}

		public void Add(object key, object value)
		{
			((IDictionary)Contents).Add(key, value);
		}

		public void Clear()
		{
			Contents.Clear();
		}

		public bool Contains(object key)
		{
			return ((IDictionary)Contents).Contains(key);
		}

		public void CopyTo(Array array, int index)
		{
			((ICollection)Contents).CopyTo(array, index);
		}

		public IDictionaryEnumerator GetEnumerator()
		{
			return ((IDictionary)Contents).GetEnumerator();
		}

		public void Remove(object key)
		{
			((IDictionary)Contents).Remove(key);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IDictionary)Contents).GetEnumerator();
		}

		public T Get<T>(object key)
		{
			if (typeof(T) == typeof(NTROArray))
			{
				return (T)this[key];
			}
			return ((NTROValue<T>)this[key]).Value;
		}
	}
}
