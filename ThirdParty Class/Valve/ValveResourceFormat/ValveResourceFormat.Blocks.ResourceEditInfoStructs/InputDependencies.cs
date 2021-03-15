using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ValveResourceFormat.Blocks.ResourceEditInfoStructs
{
	public class InputDependencies : REDIBlock
	{
		public class InputDependency
		{
			public string ContentRelativeFilename
			{
				get;
				set;
			}

			public string ContentSearchPath
			{
				get;
				set;
			}

			public uint FileCRC
			{
				get;
				set;
			}

			public uint Flags
			{
				get;
				set;
			}

			public void WriteText(IndentedTextWriter writer)
			{
				writer.WriteLine("ResourceInputDependency_t");
				writer.WriteLine("{");
				checked
				{
					writer.Indent++;
					writer.WriteLine("CResourceString m_ContentRelativeFilename = \"{0}\"", ContentRelativeFilename);
					writer.WriteLine("CResourceString m_ContentSearchPath = \"{0}\"", ContentSearchPath);
					writer.WriteLine("uint32 m_nFileCRC = 0x{0:X8}", FileCRC);
					writer.WriteLine("uint32 m_nFlags = 0x{0:X8}", Flags);
					writer.Indent--;
					writer.WriteLine("}");
				}
			}
		}

		public List<InputDependency> List
		{
			get;
		}

		public InputDependencies()
		{
			List = new List<InputDependency>();
		}

		public override void Read(BinaryReader reader, Resource resource)
		{
			reader.BaseStream.Position = base.Offset;
			for (int i = 0; i < base.Size; i = checked(i + 1))
			{
				InputDependency inputDependency = new InputDependency();
				inputDependency.ContentRelativeFilename = reader.ReadOffsetString(Encoding.UTF8);
				inputDependency.ContentSearchPath = reader.ReadOffsetString(Encoding.UTF8);
				inputDependency.FileCRC = reader.ReadUInt32();
				inputDependency.Flags = reader.ReadUInt32();
				List.Add(inputDependency);
			}
		}

		public override void WriteText(IndentedTextWriter writer)
		{
			writer.WriteLine("Struct m_InputDependencies[{0}] =", List.Count);
			WriteList(writer);
		}

		protected void WriteList(IndentedTextWriter writer)
		{
			writer.WriteLine("[");
			checked
			{
				writer.Indent++;
				foreach (InputDependency item in List)
				{
					item.WriteText(writer);
				}
				writer.Indent--;
				writer.WriteLine("]");
			}
		}
	}
}
