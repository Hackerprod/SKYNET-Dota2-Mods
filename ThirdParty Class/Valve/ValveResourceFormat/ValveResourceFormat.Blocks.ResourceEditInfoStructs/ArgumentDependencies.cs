using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ValveResourceFormat.Blocks.ResourceEditInfoStructs
{
	public class ArgumentDependencies : REDIBlock
	{
		public class ArgumentDependency
		{
			public string ParameterName
			{
				get;
				set;
			}

			public string ParameterType
			{
				get;
				set;
			}

			public uint Fingerprint
			{
				get;
				set;
			}

			public uint FingerprintDefault
			{
				get;
				set;
			}

			public void WriteText(IndentedTextWriter writer)
			{
				writer.WriteLine("ResourceArgumentDependency_t");
				writer.WriteLine("{");
				checked
				{
					writer.Indent++;
					writer.WriteLine("CResourceString m_ParameterName = \"{0}\"", ParameterName);
					writer.WriteLine("CResourceString m_ParameterType = \"{0}\"", ParameterType);
					writer.WriteLine("uint32 m_nFingerprint = 0x{0:X8}", Fingerprint);
					writer.WriteLine("uint32 m_nFingerprintDefault = 0x{0:X8}", FingerprintDefault);
					writer.Indent--;
					writer.WriteLine("}");
				}
			}
		}

		public List<ArgumentDependency> List
		{
			get;
		}

		public ArgumentDependencies()
		{
			List = new List<ArgumentDependency>();
		}

		public override void Read(BinaryReader reader, Resource resource)
		{
			reader.BaseStream.Position = base.Offset;
			for (int i = 0; i < base.Size; i = checked(i + 1))
			{
				ArgumentDependency argumentDependency = new ArgumentDependency();
				argumentDependency.ParameterName = reader.ReadOffsetString(Encoding.UTF8);
				argumentDependency.ParameterType = reader.ReadOffsetString(Encoding.UTF8);
				argumentDependency.Fingerprint = reader.ReadUInt32();
				argumentDependency.FingerprintDefault = reader.ReadUInt32();
				List.Add(argumentDependency);
			}
		}

		public override void WriteText(IndentedTextWriter writer)
		{
			writer.WriteLine("Struct m_ArgumentDependencies[{0}] =", List.Count);
			writer.WriteLine("[");
			checked
			{
				writer.Indent++;
				foreach (ArgumentDependency item in List)
				{
					item.WriteText(writer);
				}
				writer.Indent--;
				writer.WriteLine("]");
			}
		}
	}
}
