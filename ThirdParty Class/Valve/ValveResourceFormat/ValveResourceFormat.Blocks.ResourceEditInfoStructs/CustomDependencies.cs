using System;
using System.CodeDom.Compiler;
using System.IO;

namespace ValveResourceFormat.Blocks.ResourceEditInfoStructs
{
	public class CustomDependencies : REDIBlock
	{
		public override void Read(BinaryReader reader, Resource resource)
		{
			reader.BaseStream.Position = base.Offset;
			if (base.Size != 0)
			{
				throw new NotImplementedException("CustomDependencies block is not handled. Please report this on https://github.com/SteamDatabase/ValveResourceFormat and provide the file that caused this exception.");
			}
		}

		public override void WriteText(IndentedTextWriter writer)
		{
			writer.WriteLine("Struct m_CustomDependencies[{0}] =", 0);
			writer.WriteLine("[");
			checked
			{
				writer.Indent++;
				writer.Indent--;
				writer.WriteLine("]");
			}
		}
	}
}
