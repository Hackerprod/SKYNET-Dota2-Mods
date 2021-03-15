using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ValveResourceFormat.Blocks.ResourceEditInfoStructs
{
	public class ExtraIntData : REDIBlock
	{
		public class EditIntData
		{
			public string Name
			{
				get;
				set;
			}

			public int Value
			{
				get;
				set;
			}

			public void WriteText(IndentedTextWriter writer)
			{
				writer.WriteLine("ResourceEditIntData_t");
				writer.WriteLine("{");
				checked
				{
					writer.Indent++;
					writer.WriteLine("CResourceString m_Name = \"{0}\"", Name);
					writer.WriteLine("int32 m_nInt = {0}", Value);
					writer.Indent--;
					writer.WriteLine("}");
				}
			}
		}

		public List<EditIntData> List
		{
			get;
		}

		public ExtraIntData()
		{
			List = new List<EditIntData>();
		}

		public override void Read(BinaryReader reader, Resource resource)
		{
			reader.BaseStream.Position = base.Offset;
			for (int i = 0; i < base.Size; i = checked(i + 1))
			{
				EditIntData editIntData = new EditIntData();
				editIntData.Name = reader.ReadOffsetString(Encoding.UTF8);
				editIntData.Value = reader.ReadInt32();
				List.Add(editIntData);
			}
		}

		public override void WriteText(IndentedTextWriter writer)
		{
			writer.WriteLine("Struct m_ExtraIntData[{0}] =", List.Count);
			writer.WriteLine("[");
			checked
			{
				writer.Indent++;
				foreach (EditIntData item in List)
				{
					item.WriteText(writer);
				}
				writer.Indent--;
				writer.WriteLine("]");
			}
		}
	}
}
