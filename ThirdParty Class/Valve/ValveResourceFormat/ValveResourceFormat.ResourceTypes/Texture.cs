using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using ValveResourceFormat.Blocks;
using ValveResourceFormat.Blocks.ResourceEditInfoStructs;
using ValveResourceFormat.ThirdParty;

namespace ValveResourceFormat.ResourceTypes
{
	public class Texture : ResourceData
	{
		private BinaryReader Reader;

		private long DataOffset;

		private Resource Resource;

		public ushort Version
		{
			get;
			private set;
		}

		public ushort Width
		{
			get;
			private set;
		}

		public ushort Height
		{
			get;
			private set;
		}

		public ushort Depth
		{
			get;
			private set;
		}

		public float[] Reflectivity
		{
			get;
			private set;
		}

		public VTexFlags Flags
		{
			get;
			private set;
		}

		public VTexFormat Format
		{
			get;
			private set;
		}

		public byte NumMipLevels
		{
			get;
			private set;
		}

		public uint Picmip0Res
		{
			get;
			private set;
		}

		public Dictionary<VTexExtraData, byte[]> ExtraData
		{
			get;
			private set;
		}

		public Texture()
		{
			ExtraData = new Dictionary<VTexExtraData, byte[]>();
		}

		public override void Read(BinaryReader reader, Resource resource)
		{
			Reader = reader;
			Resource = resource;
			reader.BaseStream.Position = base.Offset;
			Version = reader.ReadUInt16();
			if (Version != 1)
			{
				throw new InvalidDataException($"Unknown vtex version. ({Version} != expected 1)");
			}
			Flags = (VTexFlags)reader.ReadUInt16();
			Reflectivity = new float[4]
			{
				reader.ReadSingle(),
				reader.ReadSingle(),
				reader.ReadSingle(),
				reader.ReadSingle()
			};
			Width = reader.ReadUInt16();
			Height = reader.ReadUInt16();
			Depth = reader.ReadUInt16();
			Format = (VTexFormat)reader.ReadByte();
			NumMipLevels = reader.ReadByte();
			Picmip0Res = reader.ReadUInt32();
			uint num = reader.ReadUInt32();
			uint num2 = reader.ReadUInt32();
			checked
			{
				if (num2 != 0)
				{
					reader.BaseStream.Position += num - 8u;
					for (int i = 0; i < num2; i++)
					{
						uint num3 = reader.ReadUInt32();
						uint num4 = reader.ReadUInt32() - 8u;
						uint num5 = reader.ReadUInt32();
						long position = reader.BaseStream.Position;
						reader.BaseStream.Position += num4;
						ExtraData.Add(unchecked((VTexExtraData)checked((int)num3)), reader.ReadBytes((int)num5));
						reader.BaseStream.Position = position;
					}
				}
				DataOffset = base.Offset + base.Size;
			}
		}

		public Bitmap GenerateBitmap()
		{
			Reader.BaseStream.Position = DataOffset;
			switch (Format)
			{
			case VTexFormat.RGBA8888:
			{
				ushort num3 = 0;
				checked
				{
					if (num3 < Depth && num3 < 255)
					{
						byte b3 = NumMipLevels;
						while (b3 > 0 && b3 != 1)
						{
							for (int m = 0; unchecked((double)m < (double)(int)Height / Math.Pow(2.0, (double)checked(unchecked((int)b3) - 1))); m++)
							{
								Reader.BaseStream.Position += (int)unchecked((double)checked(4 * unchecked((int)Width)) / Math.Pow(2.0, (double)checked(unchecked((int)b3) - 1)));
							}
							b3 = (byte)(unchecked((uint)b3) - 1u);
						}
						return ReadRGBA8888(Reader, Width, Height);
					}
					break;
				}
			}
			case VTexFormat.RGBA16161616F:
				return ReadRGBA16161616F(Reader, Width, Height);
			case VTexFormat.DXT1:
			{
				ushort num2 = 0;
				if (num2 < Depth && num2 < 255)
				{
					byte b2 = NumMipLevels;
					while (b2 > 0 && b2 != 1)
					{
						for (int k = 0; (double)k < (double)(int)Height / Math.Pow(2.0, (double)checked(unchecked((int)b2) + 1)); k = checked(k + 1))
						{
							for (int l = 0; (double)l < (double)(int)Width / Math.Pow(2.0, (double)checked(unchecked((int)b2) + 1)); l = checked(l + 1))
							{
								checked
								{
									Reader.BaseStream.Position += 8L;
								}
							}
						}
						checked
						{
							b2 = (byte)(unchecked((uint)b2) - 1u);
						}
					}
					return DDSImage.UncompressDXT1(Reader, Width, Height);
				}
				break;
			}
			case VTexFormat.DXT5:
			{
				bool yCoCg = false;
				if (Resource.EditInfo.Structs.ContainsKey(ResourceEditInfo.REDIStruct.SpecialDependencies))
				{
					SpecialDependencies specialDependencies = (SpecialDependencies)Resource.EditInfo.Structs[ResourceEditInfo.REDIStruct.SpecialDependencies];
					foreach (SpecialDependencies.SpecialDependency item in specialDependencies.List)
					{
						if (item.CompilerIdentifier == "CompileTexture" && item.String == "Texture Compiler Version Image YCoCg Conversion")
						{
							yCoCg = true;
							break;
						}
					}
				}
				ushort num = 0;
				if (num < Depth && num < 255)
				{
					byte b = NumMipLevels;
					while (b > 0 && b != 1)
					{
						for (int i = 0; (double)i < (double)(int)Height / Math.Pow(2.0, (double)checked(unchecked((int)b) + 1)); i = checked(i + 1))
						{
							for (int j = 0; (double)j < (double)(int)Width / Math.Pow(2.0, (double)checked(unchecked((int)b) + 1)); j = checked(j + 1))
							{
								checked
								{
									Reader.BaseStream.Position += 16L;
								}
							}
						}
						checked
						{
							b = (byte)(unchecked((uint)b) - 1u);
						}
					}
					return DDSImage.UncompressDXT5(Reader, Width, Height, yCoCg);
				}
				break;
			}
			case VTexFormat.PNG:
			case (VTexFormat)17:
			case (VTexFormat)18:
				return ReadPNG();
			}
			throw new NotImplementedException($"Unhandled image type: {Format}");
		}

		private Bitmap ReadPNG()
		{
			using (MemoryStream stream = new MemoryStream(Reader.ReadBytes(checked((int)Reader.BaseStream.Length))))
			{
				return new Bitmap(Image.FromStream(stream));
			}
		}

		private static Bitmap ReadRGBA8888(BinaryReader r, int w, int h)
		{
			Bitmap bitmap = new Bitmap(w, h);
			checked
			{
				for (int i = 0; i < h; i++)
				{
					for (int j = 0; j < w; j++)
					{
						int num = r.ReadInt32();
						Color color = Color.FromArgb((num >> 24) & 0xFF, num & 0xFF, (num >> 8) & 0xFF, (num >> 16) & 0xFF);
						bitmap.SetPixel(j, i, color);
					}
				}
				return bitmap;
			}
		}

		private static Bitmap ReadRGBA16161616F(BinaryReader r, int w, int h)
		{
			Bitmap bitmap = new Bitmap(w, h);
			checked
			{
				while (h-- > 0)
				{
					while (w-- > 0)
					{
						int red = (int)r.ReadDouble();
						int green = (int)r.ReadDouble();
						int blue = (int)r.ReadDouble();
						int alpha = (int)r.ReadDouble();
						bitmap.SetPixel(w, h, Color.FromArgb(alpha, red, green, blue));
					}
				}
				return bitmap;
			}
		}

		public override string ToString()
		{
			using (StringWriter stringWriter = new StringWriter())
			{
				using (IndentedTextWriter indentedTextWriter = new IndentedTextWriter(stringWriter, "\t"))
				{
					indentedTextWriter.WriteLine("{0,-12} = {1}", "VTEX Version", Version);
					indentedTextWriter.WriteLine("{0,-12} = {1}", "Width", Width);
					indentedTextWriter.WriteLine("{0,-12} = {1}", "Height", Height);
					indentedTextWriter.WriteLine("{0,-12} = {1}", "Depth", Depth);
					indentedTextWriter.WriteLine("{0,-12} = ( {1:F6}, {2:F6}, {3:F6}, {4:F6} )", "Reflectivity", Reflectivity[0], Reflectivity[1], Reflectivity[2], Reflectivity[3]);
					indentedTextWriter.WriteLine("{0,-12} = {1}", "NumMipLevels", NumMipLevels);
					indentedTextWriter.WriteLine("{0,-12} = {1}", "Picmip0Res", Picmip0Res);
					indentedTextWriter.WriteLine("{0,-12} = {1} (VTEX_FORMAT_{2})", "Format", (int)Format, Format);
					indentedTextWriter.WriteLine("{0,-12} = 0x{1:X8}", "Flags", (int)Flags);
					foreach (Enum value in Enum.GetValues(Flags.GetType()))
					{
						if (Flags.HasFlag(value))
						{
							indentedTextWriter.WriteLine("{0,-12} | 0x{1:X8} = VTEX_FLAG_{2}", string.Empty, Convert.ToInt32(value), value);
						}
					}
					indentedTextWriter.WriteLine("{0,-12} = {1} entries:", "Extra Data", ExtraData.Count);
					int num = 0;
					foreach (KeyValuePair<VTexExtraData, byte[]> extraDatum in ExtraData)
					{
						indentedTextWriter.WriteLine("{0,-12}   [ Entry {1}: VTEX_EXTRA_DATA_{2} - {3} bytes ]", string.Empty, num++, extraDatum.Key, extraDatum.Value.Length);
					}
					return stringWriter.ToString();
				}
			}
		}
	}
}
