using SkiaSharp.Views.Desktop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using ValveResourceFormat;
using ValveResourceFormat.ResourceTypes;

namespace Dota2
{
	public class ExtractEconItemsCommand
	{
		public string Description
		{
			get;
		}

		public string Usage
		{
			get;
		}

		public string Name
		{
			get;
		}

		public string Module
		{
			get;
		}

		public void ProcessCommand()
		{
            string text = Path.Combine(@"D:\Juegos\Steam\Steam\steamapps\common\dota 2 beta\game\dota");
            string text2 = Path.Combine(text, "pak01_dir.vpk");
            string text3 = Path.Combine(Paths.WwwBasePath, "cdn.dota2.com", "apps", "570", "images");
            Paths.EnsureDirectory(text3);
            if (Directory.Exists(text) && File.Exists(text2))
            {
                modCommon.WriteLine("Extracting econ images from dota");
                try
                {
                    Package val = new Package();
                    try
                    {
                        val.Read(text2);
                        List<PackageEntry> list = (from e in val.Entries["vtex_c"]
                                                   where e.DirectoryName.Contains("panorama/images/econ")
                                                   select e).ToList();
                        if (list.Any())
                        {
                            modCommon.WriteLine($"{list.Count} Econ items found.");
                            int num = 0;
                            foreach (PackageEntry item in list)
                            {
                                string text4 = Path.Combine(text3, item.DirectoryName.Replace("panorama/images/", string.Empty));
                                string text5 = Path.Combine(text4, item.FileName.Replace("_png", string.Empty) + ".png");
                                num++;
                                if (num % 1000 == 0)
                                {
                                    modCommon.Write($"{(float)num / ((float)list.Count * 1f) * 100f:0}% generated.\r\n");
                                }
                                if (!File.Exists(text5))
                                {
                                    Paths.EnsureDirectory(text4);
                                    byte[] bytes = default(byte[]);
                                    val.ReadEntry(item, out bytes, false);
                                    Resource val2 = new Resource();
                                    try
                                    {
                                        using (MemoryStream memoryStream = RecyclableStreams.Create(bytes))
                                        {
                                            val2.Read((Stream)memoryStream);
                                            try
                                            {
                                                using (Bitmap bitmap = ((Texture)val2.Blocks[BlockType.DATA]).GenerateBitmap().ToBitmap())
                                                {
                                                    if (bitmap.Size.Width == 256 && bitmap.Size.Height == 256)
                                                    {
                                                        using (Bitmap bitmap2 = new Bitmap(256, 170))
                                                        {
                                                            using (Graphics graphics = Graphics.FromImage(bitmap2))
                                                            {
                                                                graphics.DrawImage(bitmap, new Rectangle(0, 0, bitmap2.Width, bitmap2.Height), new Rectangle(0, 0, bitmap2.Width, bitmap2.Height), GraphicsUnit.Pixel);
                                                                graphics.Save();
                                                            }
                                                            bitmap2.Save(text5, ImageFormat.Png);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        bitmap.Save(text5, ImageFormat.Png);
                                                    }
                                                }
                                            }
                                            catch (Exception)
                                            {

                                            }
                                        }
                                    }
                                    finally
                                    {
                                        ((IDisposable)val2)?.Dispose();
                                    }
                                }
                            }
                            modCommon.Write($"{(float)num / ((float)list.Count * 1f) * 100f:0}% generated.\r\n");
                        }
                    }
                    finally
                    {
                        ((IDisposable)val)?.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    modCommon.WriteLine("Error extracting econ images.\r\n" + ex.Message + "\r\n" + ex.StackTrace);
                    return;
                }
                modCommon.WriteLine("Econ images was extracted successfully.");
            }
            else
            {
                modCommon.WriteLine("Dota 2 was not found on content folders " + text + "...");
            }
        }
	}
}
