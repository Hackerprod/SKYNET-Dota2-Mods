using System.IO;
using ValveResourceFormat.Blocks;

namespace ValveResourceFormat.ResourceTypes
{
	public class Particle : NTRO
	{
		private const int SIGNATURE = 55987030;

		public override void Read(BinaryReader reader, Resource resource)
		{
			if (resource.IntrospectionManifest == null)
			{
				ResourceIntrospectionManifest.ResourceDiskStruct resourceDiskStruct = new ResourceIntrospectionManifest.ResourceDiskStruct();
				ResourceIntrospectionManifest.ResourceDiskStruct.Field item = new ResourceIntrospectionManifest.ResourceDiskStruct.Field
				{
					FieldName = "m_Signature",
					Count = 1,
					Type = DataType.Int32
				};
				resourceDiskStruct.FieldIntrospection.Add(item);
				item = new ResourceIntrospectionManifest.ResourceDiskStruct.Field
				{
					FieldName = "m_Encoding",
					Count = 4,
					OnDiskOffset = 4,
					Type = DataType.Boolean
				};
				resourceDiskStruct.FieldIntrospection.Add(item);
				item = new ResourceIntrospectionManifest.ResourceDiskStruct.Field
				{
					FieldName = "m_Format",
					Count = 4,
					OnDiskOffset = 20,
					Type = DataType.Boolean
				};
				resourceDiskStruct.FieldIntrospection.Add(item);
				resource.Blocks[BlockType.NTRO] = new ResourceIntrospectionManifest();
				resource.IntrospectionManifest.ReferencedStructs.Add(resourceDiskStruct);
			}
			base.Read(reader, resource);
			reader.BaseStream.Position = base.Offset;
			if (reader.ReadUInt32() != 55987030)
			{
				throw new InvalidDataException("Wrong signature.");
			}
			checked
			{
				reader.BaseStream.Position += 32L;
			}
		}

		public override string ToString()
		{
			return "particle";
		}
	}
}
