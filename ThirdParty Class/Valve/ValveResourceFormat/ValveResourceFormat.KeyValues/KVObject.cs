using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ValveResourceFormat.KeyValues
{
	public class KVObject
	{
		private bool IsArray;

		public string Key
		{
			get;
			private set;
		}

		public Dictionary<string, KVValue> Properties
		{
			get;
			private set;
		}

		public int Count
		{
			get;
			private set;
		}

		public KVObject(string name)
		{
			Key = name;
			Properties = new Dictionary<string, KVValue>();
			IsArray = false;
			Count = 0;
		}

		public KVObject(string name, bool isArray)
			: this(name)
		{
			IsArray = isArray;
		}

		public virtual void AddProperty(string name, KVValue value)
		{
			if (IsArray)
			{
				Properties.Add(Count.ToString(), value);
			}
			else
			{
				Properties.Add(name, value);
			}
			checked
			{
				Count++;
			}
		}

		public void Serialize(IndentedTextWriter writer)
		{
			if (IsArray)
			{
				SerializeArray(writer);
			}
			else
			{
				SerializeObject(writer);
			}
		}

		private void SerializeObject(IndentedTextWriter writer)
		{
			if (Key != null)
			{
				writer.WriteLine();
			}
			writer.WriteLine("{");
			checked
			{
				writer.Indent++;
				foreach (KeyValuePair<string, KVValue> property in Properties)
				{
					writer.Write(property.Key);
					writer.Write(" = ");
					PrintValue(writer, property.Value);
					writer.WriteLine();
				}
				writer.Indent--;
				writer.Write("}");
			}
		}

		private void SerializeArray(IndentedTextWriter writer)
		{
			writer.WriteLine();
			writer.WriteLine("[");
			checked
			{
				writer.Indent++;
				for (int i = 0; i < Count; i++)
				{
					PrintValue(writer, Properties[i.ToString()]);
					writer.WriteLine(",");
				}
				writer.Indent--;
				writer.Write("]");
			}
		}

		private string EscapeUnescaped(string input, char toEscape)
		{
			if (input.Length == 0)
			{
				return input;
			}
			int startIndex = 1;
			checked
			{
				while (true)
				{
					startIndex = input.IndexOf(toEscape, startIndex);
					if (startIndex == -1)
					{
						break;
					}
					if (input.ElementAt(startIndex - 1) != '\\')
					{
						input = input.Insert(startIndex, "\\");
					}
					startIndex++;
				}
				return input;
			}
		}

		private void PrintValue(IndentedTextWriter writer, KVValue kvValue)
		{
			KVType type = kvValue.Type;
			object value = kvValue.Value;
			KVFlaggedValue kVFlaggedValue = kvValue as KVFlaggedValue;
			if (kVFlaggedValue != null)
			{
				switch (kVFlaggedValue.Flag)
				{
				case KVFlag.Resource:
					writer.Write("resource:");
					break;
				case KVFlag.DeferredResource:
					writer.Write("deferred_resource:");
					break;
				default:
					throw new InvalidOperationException("Trying to print unknown flag");
				}
			}
			switch (type)
			{
			case KVType.ARRAY:
			case KVType.OBJECT:
				((KVObject)value).Serialize(writer);
				break;
			case KVType.FLAGGED_STRING:
				writer.Write((string)value);
				break;
			case KVType.STRING:
				writer.Write("\"");
				writer.Write(EscapeUnescaped((string)value, '"'));
				writer.Write("\"");
				break;
			case KVType.STRING_MULTI:
				writer.Write("\"\"\"\n");
				writer.Write((string)value);
				writer.Write("\n\"\"\"");
				break;
			case KVType.BOOLEAN:
				writer.Write(((bool)value) ? "true" : "false");
				break;
			case KVType.DOUBLE:
				writer.Write(((double)value).ToString("#0.000000", CultureInfo.InvariantCulture));
				break;
			case KVType.INTEGER:
				writer.Write((long)value);
				break;
			case KVType.NULL:
				writer.Write("null");
				break;
			default:
				throw new InvalidOperationException("Trying to print unknown type.");
			}
		}
	}
}
