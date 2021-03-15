using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace ValveResourceFormat.ThirdParty
{
	internal static class DDSImage
	{
		public static Bitmap UncompressDXT1(BinaryReader r, int w, int h)
		{
			Rectangle rect = new Rectangle(0, 0, w, h);
			Bitmap bitmap = new Bitmap(w, h, PixelFormat.Format32bppRgb);
			int num = checked(w + 3) / 4;
			int num2 = checked(h + 3) / 4;
			BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, bitmap.PixelFormat);
			checked
			{
				byte[] pixels = new byte[bitmapData.Stride * bitmapData.Height];
				for (int i = 0; i < num2; i++)
				{
					for (int j = 0; j < num; j++)
					{
						byte[] blockStorage = r.ReadBytes(8);
						DecompressBlockDXT1(j * 4, i * 4, w, blockStorage, ref pixels, bitmapData.Stride);
					}
				}
				Marshal.Copy(pixels, 0, bitmapData.Scan0, pixels.Length);
				bitmap.UnlockBits(bitmapData);
				return bitmap;
			}
		}

		private static void DecompressBlockDXT1(int x, int y, int width, byte[] blockStorage, ref byte[] pixels, int stride)
		{
			ushort num;
			ushort num2;
			byte b;
			byte b2;
			byte b3;
			byte b4;
			byte b5;
			byte b6;
			uint num4;
			checked
			{
				num = (ushort)(blockStorage[0] | (blockStorage[1] << 8));
				num2 = (ushort)(blockStorage[2] | (blockStorage[3] << 8));
				int num3 = (num >> 11) * 255 + 16;
				b = (byte)unchecked(checked(unchecked(num3 / 32) + num3) / 32);
				num3 = ((num & 0x7E0) >> 5) * 255 + 32;
				b2 = (byte)unchecked(checked(unchecked(num3 / 64) + num3) / 64);
				num3 = (num & 0x1F) * 255 + 16;
				b3 = (byte)unchecked(checked(unchecked(num3 / 32) + num3) / 32);
				num3 = (num2 >> 11) * 255 + 16;
				b4 = (byte)unchecked(checked(unchecked(num3 / 32) + num3) / 32);
				num3 = ((num2 & 0x7E0) >> 5) * 255 + 32;
				b5 = (byte)unchecked(checked(unchecked(num3 / 64) + num3) / 64);
				num3 = (num2 & 0x1F) * 255 + 16;
				b6 = (byte)unchecked(checked(unchecked(num3 / 32) + num3) / 32);
				num4 = blockStorage[4];
			}
			uint num5 = (uint)(blockStorage[5] << 8);
			uint num6 = (uint)(blockStorage[6] << 16);
			uint num7 = (uint)(blockStorage[7] << 24);
			uint num8 = num4 | num5 | num6 | num7;
			checked
			{
				for (int i = 0; i < 4; i++)
				{
					for (int j = 0; j < 4; j++)
					{
						byte b7 = (byte)((num8 >> 2 * (4 * i + j)) & 3);
						byte b8 = 0;
						byte b9 = 0;
						byte b10 = 0;
						switch (b7)
						{
						case 0:
							b8 = b;
							b9 = b2;
							b10 = b3;
							break;
						case 1:
							b8 = b4;
							b9 = b5;
							b10 = b6;
							break;
						case 2:
							if (num > num2)
							{
								b8 = (byte)unchecked(checked(2 * unchecked((int)b) + unchecked((int)b4)) / 3);
								b9 = (byte)unchecked(checked(2 * unchecked((int)b2) + unchecked((int)b5)) / 3);
								b10 = (byte)unchecked(checked(2 * unchecked((int)b3) + unchecked((int)b6)) / 3);
							}
							else
							{
								b8 = (byte)unchecked(checked(unchecked((int)b) + unchecked((int)b4)) / 2);
								b9 = (byte)unchecked(checked(unchecked((int)b2) + unchecked((int)b5)) / 2);
								b10 = (byte)unchecked(checked(unchecked((int)b3) + unchecked((int)b6)) / 2);
							}
							break;
						case 3:
							if (num >= num2)
							{
								b8 = (byte)unchecked(checked(2 * unchecked((int)b4) + unchecked((int)b)) / 3);
								b9 = (byte)unchecked(checked(2 * unchecked((int)b5) + unchecked((int)b2)) / 3);
								b10 = (byte)unchecked(checked(2 * unchecked((int)b6) + unchecked((int)b3)) / 3);
							}
							break;
						}
						if (x + j < width)
						{
							int num9 = (y + i) * stride + (x + j) * 4;
							pixels[num9] = b10;
							pixels[num9 + 1] = b9;
							pixels[num9 + 2] = b8;
						}
					}
				}
			}
		}

		public static Bitmap UncompressDXT5(BinaryReader r, int w, int h, bool yCoCg)
		{
			Rectangle rect = new Rectangle(0, 0, w, h);
			Bitmap bitmap = new Bitmap(w, h, PixelFormat.Format32bppArgb);
			int num = checked(w + 3) / 4;
			int num2 = checked(h + 3) / 4;
			BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, bitmap.PixelFormat);
			checked
			{
				byte[] pixels = new byte[bitmapData.Stride * bitmapData.Height];
				for (int i = 0; i < num2; i++)
				{
					for (int j = 0; j < num; j++)
					{
						byte[] blockStorage = r.ReadBytes(16);
						DecompressBlockDXT5(j * 4, i * 4, w, blockStorage, ref pixels, bitmapData.Stride, yCoCg);
					}
				}
				Marshal.Copy(pixels, 0, bitmapData.Scan0, pixels.Length);
				bitmap.UnlockBits(bitmapData);
				return bitmap;
			}
		}

		private static void DecompressBlockDXT5(int x, int y, int width, byte[] blockStorage, ref byte[] pixels, int stride, bool yCoCg)
		{
			byte b = blockStorage[0];
			byte b2 = blockStorage[1];
			uint num = blockStorage[4];
			uint num2 = (uint)(blockStorage[5] << 8);
			uint num3 = (uint)(blockStorage[6] << 16);
			uint num4 = (uint)(blockStorage[7] << 24);
			uint num5 = num | num2 | num3 | num4;
			ushort num6;
			byte b3;
			byte b4;
			byte b5;
			byte b6;
			byte b7;
			byte b8;
			uint num10;
			checked
			{
				num6 = (ushort)(blockStorage[2] | (blockStorage[3] << 8));
				ushort num7 = (ushort)(blockStorage[8] | (blockStorage[9] << 8));
				ushort num8 = (ushort)(blockStorage[10] | (blockStorage[11] << 8));
				int num9 = (num7 >> 11) * 255 + 16;
				b3 = (byte)unchecked(checked(unchecked(num9 / 32) + num9) / 32);
				num9 = ((num7 & 0x7E0) >> 5) * 255 + 32;
				b4 = (byte)unchecked(checked(unchecked(num9 / 64) + num9) / 64);
				num9 = (num7 & 0x1F) * 255 + 16;
				b5 = (byte)unchecked(checked(unchecked(num9 / 32) + num9) / 32);
				num9 = (num8 >> 11) * 255 + 16;
				b6 = (byte)unchecked(checked(unchecked(num9 / 32) + num9) / 32);
				num9 = ((num8 & 0x7E0) >> 5) * 255 + 32;
				b7 = (byte)unchecked(checked(unchecked(num9 / 64) + num9) / 64);
				num9 = (num8 & 0x1F) * 255 + 16;
				b8 = (byte)unchecked(checked(unchecked(num9 / 32) + num9) / 32);
				num10 = blockStorage[12];
			}
			uint num11 = (uint)(blockStorage[13] << 8);
			uint num12 = (uint)(blockStorage[14] << 16);
			uint num13 = (uint)(blockStorage[15] << 24);
			uint num14 = num10 | num11 | num12 | num13;
			checked
			{
				for (int i = 0; i < 4; i++)
				{
					for (int j = 0; j < 4; j++)
					{
						int num15 = 3 * (4 * i + j);
						int num16 = (num15 <= 12) ? ((num6 >> num15) & 7) : ((num15 != 15) ? ((int)((num5 >> num15 - 16) & 7)) : ((int)((unchecked((uint)num6) >> 15) | ((num5 << 1) & 6))));
						byte b9;
						switch (num16)
						{
						case 0:
							b9 = b;
							break;
						case 1:
							b9 = b2;
							break;
						default:
							if (b > b2)
							{
								b9 = (byte)unchecked(checked((8 - num16) * unchecked((int)b) + (num16 - 1) * unchecked((int)b2)) / 7);
							}
							else
							{
								switch (num16)
								{
								case 6:
									b9 = 0;
									break;
								case 7:
									b9 = byte.MaxValue;
									break;
								default:
									b9 = (byte)unchecked(checked((6 - num16) * unchecked((int)b) + (num16 - 1) * unchecked((int)b2)) / 5);
									break;
								}
							}
							break;
						}
						byte b10 = (byte)((num14 >> 2 * (4 * i + j)) & 3);
						byte b11 = 0;
						byte b12 = 0;
						byte b13 = 0;
						switch (b10)
						{
						case 0:
							b11 = b3;
							b12 = b4;
							b13 = b5;
							break;
						case 1:
							b11 = b6;
							b12 = b7;
							b13 = b8;
							break;
						case 2:
							b11 = (byte)unchecked(checked(2 * unchecked((int)b3) + unchecked((int)b6)) / 3);
							b12 = (byte)unchecked(checked(2 * unchecked((int)b4) + unchecked((int)b7)) / 3);
							b13 = (byte)unchecked(checked(2 * unchecked((int)b5) + unchecked((int)b8)) / 3);
							break;
						case 3:
							b11 = (byte)unchecked(checked(2 * unchecked((int)b6) + unchecked((int)b3)) / 3);
							b12 = (byte)unchecked(checked(2 * unchecked((int)b7) + unchecked((int)b4)) / 3);
							b13 = (byte)unchecked(checked(2 * unchecked((int)b8) + unchecked((int)b5)) / 3);
							break;
						}
						if (x + j < width)
						{
							if (yCoCg)
							{
								int num17 = (b13 >> 3) + 1;
								int num18;
								int num19;
								unchecked
								{
									num18 = checked(unchecked((int)b11) - 128) / num17;
									num19 = checked(unchecked((int)b12) - 128) / num17;
								}
								b11 = ClampColor(unchecked((int)b9) + num18 - num19);
								b12 = ClampColor(unchecked((int)b9) + num19);
								b13 = ClampColor(unchecked((int)b9) - num18 - num19);
							}
							int num20 = (y + i) * stride + (x + j) * 4;
							pixels[num20] = b13;
							pixels[num20 + 1] = b12;
							pixels[num20 + 2] = b11;
							pixels[num20 + 3] = byte.MaxValue;
						}
					}
				}
			}
		}

		private static byte ClampColor(int a)
		{
			if (a > 255)
			{
				return byte.MaxValue;
			}
			if (a >= 0)
			{
				return checked((byte)a);
			}
			return 0;
		}
	}
}
