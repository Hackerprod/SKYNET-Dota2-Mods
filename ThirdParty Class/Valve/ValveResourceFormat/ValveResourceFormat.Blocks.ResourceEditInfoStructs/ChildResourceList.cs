using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ValveResourceFormat.Blocks.ResourceEditInfoStructs
{
	public class ChildResourceList : REDIBlock
	{
		public class ReferenceInfo
		{
			public ulong Id
			{
				get;
				set;
			}

			public string ResourceName
			{
				get;
				set;
			}

			public void WriteText(IndentedTextWriter writer)
			{
				writer.WriteLine("ResourceReferenceInfo_t");
				writer.WriteLine("{");
				checked
				{
					writer.Indent++;
					writer.WriteLine("uint64 m_nId = 0x{0:X16}", Id);
					writer.WriteLine("CResourceString m_pResourceName = \"{0}\"", ResourceName);
					writer.Indent--;
					writer.WriteLine("}");
				}
			}
		}

		public List<ReferenceInfo> List
		{
			get;
		}

		public ChildResourceList()
		{
			List = new List<ReferenceInfo>();
		}

		public override void Read(BinaryReader reader, Resource resource)
		{
			reader.BaseStream.Position = base.Offset;
			for (int i = 0; i < base.Size; i = checked(i + 1))
			{
				ReferenceInfo referenceInfo = new ReferenceInfo();
				referenceInfo.Id = reader.ReadUInt64();
				referenceInfo.ResourceName = reader.ReadOffsetString(Encoding.UTF8);
				reader.ReadBytes(4);
				List.Add(referenceInfo);
			}
		}

		public override void WriteText(IndentedTextWriter writer)
		{
			writer.WriteLine("Struct m_ChildResourceList[{0}] =", List.Count);
			writer.WriteLine("[");
			checked
			{
				writer.Indent++;
				foreach (ReferenceInfo item in List)
				{
					item.WriteText(writer);
				}
				writer.Indent--;
				writer.WriteLine("]");
			}
		}
	}
}
