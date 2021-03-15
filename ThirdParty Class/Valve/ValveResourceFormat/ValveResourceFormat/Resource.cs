using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ValveResourceFormat.Blocks;
using ValveResourceFormat.Blocks.ResourceEditInfoStructs;
using ValveResourceFormat.ResourceTypes;

namespace ValveResourceFormat
{
	public class Resource : IDisposable
	{
		private const ushort KnownHeaderVersion = 12;

		private FileStream FileStream;

		public BinaryReader Reader
		{
			get;
			private set;
		}

		public uint FileSize
		{
			get;
			private set;
		}

		public ushort HeaderVersion
		{
			get;
			private set;
		}

		public ushort Version
		{
			get;
			private set;
		}

		public Dictionary<BlockType, Block> Blocks
		{
			get;
		}

		public ResourceType ResourceType
		{
			get;
			set;
		}

		public ResourceExtRefList ExternalReferences
		{
			get
			{
				Blocks.TryGetValue(BlockType.RERL, out Block value);
				return (ResourceExtRefList)value;
			}
		}

		public ResourceEditInfo EditInfo
		{
			get
			{
				Blocks.TryGetValue(BlockType.REDI, out Block value);
				return (ResourceEditInfo)value;
			}
		}

		public ResourceIntrospectionManifest IntrospectionManifest
		{
			get
			{
				Blocks.TryGetValue(BlockType.NTRO, out Block value);
				return (ResourceIntrospectionManifest)value;
			}
		}

		public VBIB VBIB
		{
			get
			{
				Blocks.TryGetValue(BlockType.VBIB, out Block value);
				return (VBIB)value;
			}
		}

		public Resource()
		{
			ResourceType = ResourceType.Unknown;
			Blocks = new Dictionary<BlockType, Block>();
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (FileStream != null)
				{
					FileStream.Dispose();
					FileStream = null;
				}
				if (Reader != null)
				{
					Reader.Dispose();
					Reader = null;
				}
			}
		}

		public void Read(string filename)
		{
			FileStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
			Read(FileStream);
		}

		public void Read(Stream input)
		{
			Reader = new BinaryReader(input);
			FileSize = Reader.ReadUInt32();
			if (FileSize == 1437209140)
			{
				throw new InvalidDataException("Use Package() class to parse VPK files.");
			}
			if (FileSize == 846422902)
			{
				throw new InvalidDataException("Use CompiledShader() class to parse compiled shader files.");
			}
			uint fileSize = FileSize;
			long length = Reader.BaseStream.Length;
			HeaderVersion = Reader.ReadUInt16();
			if (HeaderVersion != 12)
			{
				throw new InvalidDataException($"Bad header version. ({HeaderVersion} != expected {(ushort)12})");
			}
			Version = Reader.ReadUInt16();
			uint num = Reader.ReadUInt32();
			uint num2 = Reader.ReadUInt32();
			checked
			{
				Reader.BaseStream.Position += num - 8u;
				for (int i = 0; i < num2; i++)
				{
					string @string = Encoding.UTF8.GetString(Reader.ReadBytes(4));
					Block block = ConstructFromType(@string);
					long position = Reader.BaseStream.Position;
					block.Offset = (uint)position + Reader.ReadUInt32();
					block.Size = Reader.ReadUInt32();
					block.Read(Reader, this);
					Blocks.Add(block.GetChar(), block);
					switch (block.GetChar())
					{
					case BlockType.REDI:
						if (ResourceType == ResourceType.Unknown && EditInfo.Structs.ContainsKey(ResourceEditInfo.REDIStruct.SpecialDependencies))
						{
							SpecialDependencies specialDependencies = unchecked((SpecialDependencies)EditInfo.Structs[ResourceEditInfo.REDIStruct.SpecialDependencies]);
							if (specialDependencies.List.Count > 0)
							{
								ResourceType = DetermineResourceTypeByCompilerIdentifier(specialDependencies.List[0]);
							}
						}
						break;
					case BlockType.NTRO:
						if (ResourceType == ResourceType.Unknown && IntrospectionManifest.ReferencedStructs.Count > 0)
						{
							string name = IntrospectionManifest.ReferencedStructs[0].Name;
							if (!(name == "VSoundEventScript_t"))
							{
								if (name == "CWorldVisibility")
								{
									ResourceType = ResourceType.WorldVisibility;
								}
							}
							else
							{
								ResourceType = ResourceType.SoundEventScript;
							}
						}
						break;
					}
					Reader.BaseStream.Position = position + 8;
				}
			}
		}

		private Block ConstructFromType(string input)
		{
			if (input == "DATA")
			{
				return ConstructResourceType();
			}
			if (input == "REDI")
			{
				return new ResourceEditInfo();
			}
			if (input == "RERL")
			{
				return new ResourceExtRefList();
			}
			if (input == "NTRO")
			{
				return new ResourceIntrospectionManifest();
			}
			if (input == "VBIB")
			{
				return new VBIB();
			}
			throw new ArgumentException($"Unrecognized block type '{input}'");
		}

		private ResourceData ConstructResourceType()
		{
			switch (ResourceType)
			{
			case ResourceType.Panorama:
			case ResourceType.PanoramaStyle:
			case ResourceType.PanoramaLayout:
			case ResourceType.PanoramaDynamicImages:
			case ResourceType.PanoramaScript:
				return new Panorama();
			case ResourceType.Sound:
				return new Sound();
			case ResourceType.Texture:
				return new Texture();
			case ResourceType.SoundEventScript:
				return new SoundEventScript();
			case ResourceType.EntityLump:
				return new EntityLump();
			case ResourceType.Particle:
				return new BinaryKV3();
			case ResourceType.Mesh:
				if (Version != 0)
				{
					return new BinaryKV3();
				}
				break;
			}
			if (Blocks.ContainsKey(BlockType.NTRO))
			{
				return new NTRO();
			}
			return new ResourceData();
		}

		private static ResourceType DetermineResourceTypeByCompilerIdentifier(SpecialDependencies.SpecialDependency input)
		{
			string text = input.CompilerIdentifier;
			if (text.StartsWith("Compile", StringComparison.Ordinal))
			{
				text = text.Remove(0, "Compile".Length);
			}
			if (text == "Psf")
			{
				return ResourceType.ParticleSnapshot;
			}
			if (text == "AnimGroup")
			{
				return ResourceType.AnimationGroup;
			}
			if (text == "VPhysXData")
			{
				return ResourceType.PhysicsCollisionMesh;
			}
			if (text == "Font")
			{
				return ResourceType.BitmapFont;
			}
			if (text == "RenderMesh")
			{
				return ResourceType.Mesh;
			}
			if (text == "Panorama")
			{
				switch (input.String)
				{
				case "Panorama Style Compiler Version":
					return ResourceType.PanoramaStyle;
				case "Panorama Script Compiler Version":
					return ResourceType.PanoramaScript;
				case "Panorama Layout Compiler Version":
					return ResourceType.PanoramaLayout;
				case "Panorama Dynamic Images Compiler Version":
					return ResourceType.PanoramaDynamicImages;
				default:
					return ResourceType.Panorama;
				}
			}
			if (Enum.TryParse(text, ignoreCase: false, out ResourceType result))
			{
				return result;
			}
			return ResourceType.Unknown;
		}
	}
}
