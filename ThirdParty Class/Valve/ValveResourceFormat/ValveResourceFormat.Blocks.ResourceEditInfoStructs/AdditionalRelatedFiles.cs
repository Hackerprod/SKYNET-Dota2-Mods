using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ValveResourceFormat.Blocks.ResourceEditInfoStructs
{
	public class AdditionalRelatedFiles : REDIBlock
	{
		public class AdditionalRelatedFile
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

			public void WriteText(IndentedTextWriter writer)
			{
				writer.WriteLine("ResourceAdditionalRelatedFile_t");
				writer.WriteLine("{");
				checked
				{
					writer.Indent++;
					writer.WriteLine("CResourceString m_ContentRelativeFilename = \"{0}\"", ContentRelativeFilename);
					writer.WriteLine("CResourceString m_ContentSearchPath = \"{0}\"", ContentSearchPath);
					writer.Indent--;
					writer.WriteLine("}");
				}
			}
		}

		public List<AdditionalRelatedFile> List
		{
			get;
		}

		public AdditionalRelatedFiles()
		{
			List = new List<AdditionalRelatedFile>();
		}

		public override void Read(BinaryReader reader, Resource resource)
		{
			reader.BaseStream.Position = base.Offset;
			for (int i = 0; i < base.Size; i = checked(i + 1))
			{
				AdditionalRelatedFile additionalRelatedFile = new AdditionalRelatedFile();
				additionalRelatedFile.ContentRelativeFilename = reader.ReadOffsetString(Encoding.UTF8);
				additionalRelatedFile.ContentSearchPath = reader.ReadOffsetString(Encoding.UTF8);
				List.Add(additionalRelatedFile);
			}
		}

		public override void WriteText(IndentedTextWriter writer)
		{
			writer.WriteLine("Struct m_AdditionalRelatedFiles[{0}] =", List.Count);
			writer.WriteLine("[");
			checked
			{
				writer.Indent++;
				foreach (AdditionalRelatedFile item in List)
				{
					item.WriteText(writer);
				}
				writer.Indent--;
				writer.WriteLine("]");
			}
		}
	}
}
