using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ValveResourceFormat.Blocks.ResourceEditInfoStructs
{
	public class ExtraStringData : REDIBlock
	{
		public class EditStringData
		{
			public string Name
			{
				get;
				set;
			}

			public string Value
			{
				get;
				set;
			}

			public void WriteText(IndentedTextWriter writer)
			{
				writer.WriteLine("ResourceEditStringData_t");
				writer.WriteLine("{");
				checked
				{
					writer.Indent++;
					writer.WriteLine("CResourceString m_Name = \"{0}\"", Name);
					string[] array = Value.Split(new string[2]
					{
						"\r\n",
						"\n"
					}, StringSplitOptions.None);
					if (array.Length > 1)
					{
						writer.Indent++;
						writer.Write("CResourceString m_String = \"");
						string[] array2 = array;
						foreach (string value in array2)
						{
							writer.WriteLine(value);
						}
						writer.WriteLine("\"");
						writer.Indent--;
					}
					else
					{
						writer.WriteLine("CResourceString m_String = \"{0}\"", Value);
					}
					writer.Indent--;
					writer.WriteLine("}");
				}
			}
		}

		public List<EditStringData> List
		{
			get;
		}

		public ExtraStringData()
		{
			List = new List<EditStringData>();
		}

		public override void Read(BinaryReader reader, Resource resource)
		{
			reader.BaseStream.Position = base.Offset;
			for (int i = 0; i < base.Size; i = checked(i + 1))
			{
				EditStringData editStringData = new EditStringData();
				editStringData.Name = reader.ReadOffsetString(Encoding.UTF8);
				editStringData.Value = reader.ReadOffsetString(Encoding.UTF8);
				List.Add(editStringData);
			}
		}

		public override void WriteText(IndentedTextWriter writer)
		{
			writer.WriteLine("Struct m_ExtraStringData[{0}] =", List.Count);
			writer.WriteLine("[");
			checked
			{
				writer.Indent++;
				foreach (EditStringData item in List)
				{
					item.WriteText(writer);
				}
				writer.Indent--;
				writer.WriteLine("]");
			}
		}
	}
}
