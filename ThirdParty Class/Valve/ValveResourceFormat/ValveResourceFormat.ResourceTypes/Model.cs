using System.IO;
using System.Text;
using ValveResourceFormat.Blocks;

namespace ValveResourceFormat.ResourceTypes
{
	public class Model : ResourceData
	{
		public override void Read(BinaryReader reader, Resource resource)
		{
			reader.BaseStream.Position = base.Offset;
			string text = reader.ReadOffsetString(Encoding.UTF8);
		}
	}
}
