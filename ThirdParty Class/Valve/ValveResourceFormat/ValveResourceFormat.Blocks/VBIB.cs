using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ValveResourceFormat.Blocks
{
	public class VBIB : Block
	{
		public struct VertexBuffer
		{
			public uint Count;

			public uint Size;

			public List<VertexAttribute> Attributes;

			public byte[] Buffer;
		}

		public struct VertexAttribute
		{
			public string Name;

			public DXGI_FORMAT Type;

			public uint Offset;
		}

		public struct IndexBuffer
		{
			public uint Count;

			public uint Size;

			public byte[] Buffer;
		}

		public List<VertexBuffer> VertexBuffers
		{
			get;
		}

		public List<IndexBuffer> IndexBuffers
		{
			get;
		}

		public VBIB()
		{
			VertexBuffers = new List<VertexBuffer>();
			IndexBuffers = new List<IndexBuffer>();
		}

		public override BlockType GetChar()
		{
			return BlockType.VBIB;
		}

		public override void Read(BinaryReader reader, Resource resource)
		{
			reader.BaseStream.Position = base.Offset;
			uint num = reader.ReadUInt32();
			uint num2 = reader.ReadUInt32();
			checked
			{
				reader.BaseStream.Position = base.Offset + num;
				for (int i = 0; i < num2; i++)
				{
					VertexBuffer vertexBuffer = default(VertexBuffer);
					vertexBuffer.Count = reader.ReadUInt32();
					vertexBuffer.Size = reader.ReadUInt32();
					long position = reader.BaseStream.Position;
					uint num3 = reader.ReadUInt32();
					uint num4 = reader.ReadUInt32();
					long position2 = reader.BaseStream.Position;
					uint num5 = reader.ReadUInt32();
					uint num6 = reader.ReadUInt32();
					vertexBuffer.Attributes = new List<VertexAttribute>();
					reader.BaseStream.Position = position + unchecked((long)num3);
					for (int j = 0; j < num4; j++)
					{
						long position3 = reader.BaseStream.Position;
						VertexAttribute item = default(VertexAttribute);
						item.Name = reader.ReadNullTermString(Encoding.UTF8);
						reader.BaseStream.Position = position3 + 36;
						unchecked
						{
							item.Type = (DXGI_FORMAT)checked((int)reader.ReadUInt32());
							item.Offset = reader.ReadUInt32();
						}
						reader.BaseStream.Position = position3 + 56;
						vertexBuffer.Attributes.Add(item);
					}
					reader.BaseStream.Position = position2 + unchecked((long)num5);
					vertexBuffer.Buffer = reader.ReadBytes((int)vertexBuffer.Count * (int)vertexBuffer.Size);
					VertexBuffers.Add(vertexBuffer);
					reader.BaseStream.Position = position2 + 4 + 4;
				}
				reader.BaseStream.Position = base.Offset + 4u + 4u;
				uint num7 = reader.ReadUInt32();
				uint num8 = reader.ReadUInt32();
				reader.BaseStream.Position = base.Offset + 8u + num7;
				for (int k = 0; k < num8; k++)
				{
					IndexBuffer indexBuffer = default(IndexBuffer);
					indexBuffer.Count = reader.ReadUInt32();
					indexBuffer.Size = reader.ReadUInt32();
					uint num9 = reader.ReadUInt32();
					uint num10 = reader.ReadUInt32();
					long position4 = reader.BaseStream.Position;
					uint num11 = reader.ReadUInt32();
					uint num12 = reader.ReadUInt32();
					reader.BaseStream.Position = position4 + unchecked((long)num11);
					indexBuffer.Buffer = reader.ReadBytes((int)indexBuffer.Count * (int)indexBuffer.Size);
					IndexBuffers.Add(indexBuffer);
					reader.BaseStream.Position = position4 + 4 + 4;
				}
			}
		}

		public override void WriteText(IndentedTextWriter writer)
		{
			writer.WriteLine("{0:X8}", base.Offset);
		}
	}
}
