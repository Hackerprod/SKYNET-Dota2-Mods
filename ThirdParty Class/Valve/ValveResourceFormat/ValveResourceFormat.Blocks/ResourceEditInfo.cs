using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using ValveResourceFormat.Blocks.ResourceEditInfoStructs;

namespace ValveResourceFormat.Blocks
{
	public class ResourceEditInfo : Block
	{
		public enum REDIStruct
		{
			InputDependencies,
			AdditionalInputDependencies,
			ArgumentDependencies,
			SpecialDependencies,
			CustomDependencies,
			AdditionalRelatedFiles,
			ChildResourceList,
			ExtraIntData,
			ExtraFloatData,
			ExtraStringData,
			End
		}

		public Dictionary<REDIStruct, REDIBlock> Structs
		{
			get;
			private set;
		}

		public ResourceEditInfo()
		{
			Structs = new Dictionary<REDIStruct, REDIBlock>();
		}

		public override BlockType GetChar()
		{
			return BlockType.REDI;
		}

		public override void Read(BinaryReader reader, Resource resource)
		{
			reader.BaseStream.Position = base.Offset;
			checked
			{
				for (REDIStruct rEDIStruct = REDIStruct.InputDependencies; rEDIStruct < REDIStruct.End; rEDIStruct++)
				{
					REDIBlock rEDIBlock = ConstructStruct(rEDIStruct);
					rEDIBlock.Offset = (uint)reader.BaseStream.Position + reader.ReadUInt32();
					rEDIBlock.Size = reader.ReadUInt32();
					Structs.Add(rEDIStruct, rEDIBlock);
				}
				foreach (KeyValuePair<REDIStruct, REDIBlock> @struct in Structs)
				{
					@struct.Value.Read(reader, resource);
				}
			}
		}

		public override void WriteText(IndentedTextWriter writer)
		{
			writer.WriteLine("ResourceEditInfoBlock_t");
			writer.WriteLine("{");
			checked
			{
				writer.Indent++;
				foreach (KeyValuePair<REDIStruct, REDIBlock> @struct in Structs)
				{
					@struct.Value.WriteText(writer);
				}
				writer.Indent--;
				writer.WriteLine("}");
			}
		}

		private static REDIBlock ConstructStruct(REDIStruct id)
		{
			switch (id)
			{
			case REDIStruct.InputDependencies:
				return new InputDependencies();
			case REDIStruct.AdditionalInputDependencies:
				return new AdditionalInputDependencies();
			case REDIStruct.ArgumentDependencies:
				return new ArgumentDependencies();
			case REDIStruct.SpecialDependencies:
				return new SpecialDependencies();
			case REDIStruct.CustomDependencies:
				return new CustomDependencies();
			case REDIStruct.AdditionalRelatedFiles:
				return new AdditionalRelatedFiles();
			case REDIStruct.ChildResourceList:
				return new ChildResourceList();
			case REDIStruct.ExtraIntData:
				return new ExtraIntData();
			case REDIStruct.ExtraFloatData:
				return new ExtraFloatData();
			case REDIStruct.ExtraStringData:
				return new ExtraStringData();
			default:
				throw new InvalidDataException("Unknown struct in REDI block.");
			}
		}
	}
}
