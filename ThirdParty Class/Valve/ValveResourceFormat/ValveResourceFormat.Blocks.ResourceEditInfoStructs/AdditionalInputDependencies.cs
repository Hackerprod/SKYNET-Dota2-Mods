using System.CodeDom.Compiler;

namespace ValveResourceFormat.Blocks.ResourceEditInfoStructs
{
	public class AdditionalInputDependencies : InputDependencies
	{
		public override void WriteText(IndentedTextWriter writer)
		{
			writer.WriteLine("Struct m_AdditionalInputDependencies[{0}] =", base.List.Count);
			WriteList(writer);
		}
	}
}
