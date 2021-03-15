using SevenZip.Compression.LZMA;
using System;
using System.IO;
using System.Text;

namespace ValveResourceFormat
{
	public class CompiledShader : IDisposable
	{
		public const int MAGIC = 846422902;

		private BinaryReader Reader;

		private string ShaderType;

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && Reader != null)
			{
				Reader.Dispose();
				Reader = null;
			}
		}

		public void Read(string filename)
		{
			FileStream input = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
			Read(filename, input);
		}

		public void Read(string filename, Stream input)
		{
			if (filename.EndsWith("vs.vcs"))
			{
				ShaderType = "vertex";
			}
			else if (filename.EndsWith("ps.vcs"))
			{
				ShaderType = "pixel";
			}
			else if (filename.EndsWith("features.vcs"))
			{
				ShaderType = "features";
			}
			Reader = new BinaryReader(input);
			if (Reader.ReadUInt32() != 846422902)
			{
				throw new InvalidDataException("Given file is not a vcs2.");
			}
			if (Reader.ReadUInt32() != 62)
			{
				throw new InvalidDataException("Not 3E.");
			}
			uint num = Reader.ReadUInt32();
			checked
			{
				if (num < 100)
				{
					Console.WriteLine("wtf: {0}", num);
					ReadFeatures();
				}
				else
				{
					Reader.BaseStream.Position -= 4L;
					ReadShader();
				}
			}
		}

		private void ReadFeatures()
		{
			string @string = Encoding.UTF8.GetString(Reader.ReadBytes(Reader.ReadInt32()));
			Reader.ReadByte();
			Console.WriteLine("Name: {0} - Offset: {1}", @string, Reader.BaseStream.Position);
			int num = Reader.ReadInt32();
			int num2 = Reader.ReadInt32();
			int num3 = Reader.ReadInt32();
			int num4 = Reader.ReadInt32();
			int num5 = Reader.ReadInt32();
			int num6 = Reader.ReadInt32();
			int num7 = Reader.ReadInt32();
			Console.WriteLine($"{num} {num2} {num3} {num4} {num5} {num6} {num7}");
			uint num8 = Reader.ReadUInt32();
			Console.WriteLine("Count: {0}", num8);
			checked
			{
				for (int i = 0; i < num8; i++)
				{
					long position = Reader.BaseStream.Position;
					@string = Reader.ReadNullTermString(Encoding.UTF8);
					Reader.BaseStream.Position = position + 128;
					uint num9 = Reader.ReadUInt32();
					Console.WriteLine("Name: {0} - Type: {1} - Offset: {2}", @string, num9, Reader.BaseStream.Position);
					if (num9 == 1)
					{
						position = Reader.BaseStream.Position;
						string value = Reader.ReadNullTermString(Encoding.UTF8);
						Console.WriteLine(value);
						Reader.BaseStream.Position = position + 64;
						Reader.ReadUInt32();
					}
				}
				for (int j = 0; j < 7; j++)
				{
					byte[] value2 = Reader.ReadBytes(16);
					Console.WriteLine("#{0} identifier: {1}", j, BitConverter.ToString(value2));
				}
				Reader.ReadUInt32();
				num8 = Reader.ReadUInt32();
				for (int k = 0; k < num8; k++)
				{
					long position = Reader.BaseStream.Position;
					@string = Reader.ReadNullTermString(Encoding.UTF8);
					Reader.BaseStream.Position = position + 64;
					position = Reader.BaseStream.Position;
					string text = Reader.ReadNullTermString(Encoding.UTF8);
					Reader.BaseStream.Position = position + 84;
					uint num10 = Reader.ReadUInt32();
					Console.WriteLine("Name: {0} - Desc: {1} - Count: {2} - Offset: {3}", @string, text, num10, Reader.BaseStream.Position);
					for (int l = 0; l < num10; l++)
					{
						Console.WriteLine("     " + Reader.ReadNullTermString(Encoding.UTF8));
					}
				}
				num8 = Reader.ReadUInt32();
			}
		}

		private void ReadShader()
		{
			byte[] value = Reader.ReadBytes(16);
			byte[] value2 = Reader.ReadBytes(16);
			Console.WriteLine("File identifier: {0}", BitConverter.ToString(value));
			Console.WriteLine("Static identifier: {0}", BitConverter.ToString(value2));
			Console.WriteLine("wtf {0}", Reader.ReadUInt32());
			uint num = Reader.ReadUInt32();
			Console.WriteLine("Count: {0} - Offset: {1}", num, Reader.BaseStream.Position);
			checked
			{
				for (int i = 0; i < num; i++)
				{
					long position = Reader.BaseStream.Position;
					string text = Reader.ReadNullTermString(Encoding.UTF8);
					Reader.BaseStream.Position = position + 128;
					int num2 = Reader.ReadInt32();
					int num3 = Reader.ReadInt32();
					int num4 = Reader.ReadInt32();
					int num5 = Reader.ReadInt32();
					int num6 = Reader.ReadInt32();
					int num7 = Reader.ReadInt32();
					Console.WriteLine($"{num2} {num3} {num4} {num5} {num6} {num7} {text}");
				}
				num = Reader.ReadUInt32();
				Console.WriteLine("Count: {0} - Offset: {1}", num, Reader.BaseStream.Position);
				for (int j = 0; j < num; j++)
				{
					Reader.BaseStream.Position += 472L;
				}
				num = Reader.ReadUInt32();
				Console.WriteLine("Count: {0} - Offset: {1}", num, Reader.BaseStream.Position);
				for (int k = 0; k < num; k++)
				{
					long position2 = Reader.BaseStream.Position;
					string text2 = Reader.ReadNullTermString(Encoding.UTF8);
					Reader.BaseStream.Position = position2 + 128;
					int num8 = Reader.ReadInt32();
					int num9 = Reader.ReadInt32();
					int num10 = Reader.ReadInt32();
					int num11 = Reader.ReadInt32();
					int num12 = Reader.ReadInt32();
					int num13 = Reader.ReadInt32();
					Console.WriteLine($"{num8} {num9} {num10} {num11} {num12} {num13} {text2}");
				}
				num = Reader.ReadUInt32();
				Console.WriteLine("Count: {0} - Offset: {1}", num, Reader.BaseStream.Position);
				for (int l = 0; l < num; l++)
				{
					Reader.BaseStream.Position += 472L;
				}
				num = Reader.ReadUInt32();
				Console.WriteLine("Count: {0} - Offset: {1}", num, Reader.BaseStream.Position);
				for (int m = 0; m < num; m++)
				{
					long position3 = Reader.BaseStream.Position;
					string arg = Reader.ReadNullTermString(Encoding.UTF8);
					Reader.BaseStream.Position = position3 + 200;
					int num14 = Reader.ReadInt32();
					int num15 = Reader.ReadInt32();
					if (num15 > -1 && num14 != 0)
					{
						Reader.BaseStream.Position = position3 + 480 + num15 + 4;
					}
					else
					{
						Reader.BaseStream.Position = position3 + 480;
					}
					Console.WriteLine($"{num14} {num15} {arg}");
				}
				num = Reader.ReadUInt32();
				Console.WriteLine("Count: {0} - Offset: {1}", num, Reader.BaseStream.Position);
				Reader.ReadBytes(280 * (int)num);
				num = Reader.ReadUInt32();
				Console.WriteLine("Count: {0} - Offset: {1}", num, Reader.BaseStream.Position);
				for (int n = 0; n < num; n++)
				{
					long position4 = Reader.BaseStream.Position;
					string text3 = Reader.ReadNullTermString(Encoding.UTF8);
					Reader.BaseStream.Position = position4 + 64;
					uint num16 = Reader.ReadUInt32();
					uint num17 = Reader.ReadUInt32();
					uint num18 = Reader.ReadUInt32();
					Console.WriteLine("[SUB CHUNK] Name: {0} - unk1: {1} - unk2: {2} - Count: {3} - Offset: {4}", text3, num16, num17, num18, Reader.BaseStream.Position);
					for (int num19 = 0; num19 < num18; num19++)
					{
						long position5 = Reader.BaseStream.Position;
						string text4 = Reader.ReadNullTermString(Encoding.UTF8);
						Reader.BaseStream.Position = position5 + 64;
						uint num20 = Reader.ReadUInt32();
						uint num21 = Reader.ReadUInt32();
						uint num22 = Reader.ReadUInt32();
						uint num23 = Reader.ReadUInt32();
						Console.WriteLine("     Name: {0} - unk1: {1} - unk2: {2} - unk3: {3} - unk4: {4}", text4, num20, num21, num22, num23);
					}
					Reader.ReadBytes(4);
				}
				Console.WriteLine("Offset: {0}", Reader.BaseStream.Position);
				if (ShaderType == "vertex")
				{
					uint num24 = Reader.ReadUInt32();
					Console.WriteLine(num24 + " vertex buffer descriptors");
					for (int num25 = 0; num25 < num24; num25++)
					{
						num = Reader.ReadUInt32();
						Console.WriteLine("Buffer #{0}, {1} attributes", num25, num);
						for (int num26 = 0; num26 < num; num26++)
						{
							string text5 = Reader.ReadNullTermString(Encoding.UTF8);
							string text6 = Reader.ReadNullTermString(Encoding.UTF8);
							string text7 = Reader.ReadNullTermString(Encoding.UTF8);
							uint num27 = Reader.ReadUInt32();
							Console.WriteLine("     Name: {0}, Type: {1}, Option: {2}, Unknown uint: {3}", text5, text6, text7, num27);
						}
					}
				}
				uint num28 = Reader.ReadUInt32();
				Console.WriteLine("Offset: {0}", Reader.BaseStream.Position);
				long[] array = new long[num28];
				for (int num29 = 0; num29 < num28; num29++)
				{
					array[num29] = Reader.ReadInt64();
				}
				int[] array2 = new int[num28];
				for (int num30 = 0; num30 < num28; num30++)
				{
					array2[num30] = Reader.ReadInt32();
				}
				for (int num31 = 0; num31 < num28; num31++)
				{
				}
			}
		}

		private byte[] ReadShaderChunk(int offset)
		{
			long position = Reader.BaseStream.Position;
			Reader.BaseStream.Position = offset;
			uint num = Reader.ReadUInt32();
			if (Reader.ReadUInt32() != 1095588428)
			{
				throw new InvalidDataException("Not LZMA?");
			}
			uint num2 = Reader.ReadUInt32();
			uint num3 = Reader.ReadUInt32();
			Console.WriteLine("Chunk size: {0}", num);
			Console.WriteLine("Compressed size: {0}", num3);
			Console.WriteLine("Uncompressed size: {0} ({1:P2} compression)", num2, (double)checked(num2 - num3) / (double)num2);
			SevenZip.Compression.LZMA.Decoder decoder = new SevenZip.Compression.LZMA.Decoder();
			decoder.SetDecoderProperties(Reader.ReadBytes(5));
			checked
			{
				byte[] array = Reader.ReadBytes((int)num3);
				Reader.BaseStream.Position = position;
				using (MemoryStream inStream = new MemoryStream(array))
				{
					using (MemoryStream memoryStream = new MemoryStream((int)num2))
					{
						decoder.Code(inStream, memoryStream, array.Length, num2, null);
						return memoryStream.ToArray();
					}
				}
			}
		}
	}
}
