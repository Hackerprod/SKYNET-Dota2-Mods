using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ValveResourceFormat.Blocks.ResourceEditInfoStructs
{
	public class SpecialDependencies : REDIBlock
	{
		public class SpecialDependency
		{
			public string String
			{
				get;
				set;
			}

			public string CompilerIdentifier
			{
				get;
				set;
			}

			public uint Fingerprint
			{
				get;
				set;
			}

			public uint UserData
			{
				get;
				set;
			}

			public void WriteText(IndentedTextWriter writer)
			{
				writer.WriteLine("ResourceSpecialDependency_t");
				writer.WriteLine("{");
				checked
				{
					writer.Indent++;
					writer.WriteLine("CResourceString m_String = \"{0}\"", String);
					writer.WriteLine("CResourceString m_CompilerIdentifier = \"{0}\"", CompilerIdentifier);
					writer.WriteLine("uint32 m_nFingerprint = 0x{0:X8}", Fingerprint);
					writer.WriteLine("uint32 m_nUserData = 0x{0:X8}", UserData);
					writer.Indent--;
					writer.WriteLine("}");
				}
			}
		}

		public List<SpecialDependency> List
		{
			get;
		}

		public SpecialDependencies()
		{
			List = new List<SpecialDependency>();
		}

		public override void Read(BinaryReader reader, Resource resource)
		{
			reader.BaseStream.Position = base.Offset;
			for (int i = 0; i < base.Size; i = checked(i + 1))
			{
				SpecialDependency specialDependency = new SpecialDependency();
				specialDependency.String = reader.ReadOffsetString(Encoding.UTF8);
				specialDependency.CompilerIdentifier = reader.ReadOffsetString(Encoding.UTF8);
				specialDependency.Fingerprint = reader.ReadUInt32();
				specialDependency.UserData = reader.ReadUInt32();
				List.Add(specialDependency);
			}
		}

		public override void WriteText(IndentedTextWriter writer)
		{
			writer.WriteLine("Struct m_SpecialDependencies[{0}] =", List.Count);
			writer.WriteLine("[");
			checked
			{
				writer.Indent++;
				foreach (SpecialDependency item in List)
				{
					item.WriteText(writer);
				}
				writer.Indent--;
				writer.WriteLine("]");
			}
		}
	}
}
