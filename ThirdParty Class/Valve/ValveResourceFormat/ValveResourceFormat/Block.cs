using System.CodeDom.Compiler;
using System.IO;

namespace ValveResourceFormat
{
	public abstract class Block
	{
		public uint Offset
		{
			get;
			set;
		}

		public uint Size
		{
			get;
			set;
		}

		public abstract BlockType GetChar();

		public abstract void Read(BinaryReader reader, Resource resource);

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

		public abstract void WriteText(IndentedTextWriter writer);
	}
}
