using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ValveResourceFormat.Blocks
{
	public class ResourceExtRefList : Block
	{
		public class ResourceReferenceInfo
		{
			public ulong Id
			{
				get;
				set;
			}

			public string Name
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
					writer.WriteLine("CResourceString m_pResourceName = \"{0}\"", Name);
					writer.Indent--;
					writer.WriteLine("}");
				}
			}
		}

		public List<ResourceReferenceInfo> ResourceRefInfoList
		{
			get;
			private set;
		}

		public string this[ulong id]
		{
			get
			{
				return ResourceRefInfoList.FirstOrDefault((ResourceReferenceInfo c) => c.Id == id)?.Name;
			}
		}

		public ResourceExtRefList()
		{
			ResourceRefInfoList = new List<ResourceReferenceInfo>();
		}

		public override BlockType GetChar()
		{
			return BlockType.RERL;
		}

		public override void Read(BinaryReader reader, Resource resource)
		{
			reader.BaseStream.Position = base.Offset;
			uint num = reader.ReadUInt32();
			uint num2 = reader.ReadUInt32();
			checked
			{
				if (num2 != 0)
				{
					reader.BaseStream.Position += num - 8u;
					for (int i = 0; i < num2; i++)
					{
						ResourceReferenceInfo resourceReferenceInfo = new ResourceReferenceInfo();
						resourceReferenceInfo.Id = reader.ReadUInt64();
						long position = reader.BaseStream.Position;
						reader.BaseStream.Position += reader.ReadInt64();
						resourceReferenceInfo.Name = reader.ReadNullTermString(Encoding.UTF8);
						ResourceRefInfoList.Add(resourceReferenceInfo);
						reader.BaseStream.Position = position + 8;
					}
				}
			}
		}

		public override void WriteText(IndentedTextWriter writer)
		{
			writer.WriteLine("ResourceExtRefList_t");
			writer.WriteLine("{");
			checked
			{
				writer.Indent++;
				writer.WriteLine("Struct m_resourceRefInfoList[{0}] =", ResourceRefInfoList.Count);
				writer.WriteLine("[");
				writer.Indent++;
				foreach (ResourceReferenceInfo resourceRefInfo in ResourceRefInfoList)
				{
					resourceRefInfo.WriteText(writer);
				}
				writer.Indent--;
				writer.WriteLine("]");
				writer.Indent--;
				writer.WriteLine("}");
			}
		}
	}
}
