using SkiaSharp.Views.Desktop;
using SkynetDota2Mods;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ValveResourceFormat;
using ValveResourceFormat.ResourceTypes;

class LoadingScreenManager
{
    internal static void Extract()
    {
        frmExportLS frmExport = new frmExportLS();
        frmExport.ShowDialog();

    }

    internal static void Extract(Items itemScreen, bool Desktop = false, bool open = false)
    {
        Rectangle workingArea1 = Screen.FromHandle(frmMain.frm.Handle).WorkingArea;
        int width = workingArea1.Width;
        int height = workingArea1.Height;

        string filename = "";
        string text3 = modCommon.DataDirectory /*+ "/Loading Screen"*/;
        Paths.EnsureDirectory(text3);

        if (modCommon.package != null)
        {
            try
            {
                try
                {
                    List<PackageEntry> list = (from l in modCommon.package.Entries["vtex_c"]
                                               where l.DirectoryName.Contains("panorama/images/loadingscreen")
                                               select l).ToList();
                    if (list.Any())
                    {
                        int num = 0;
                        foreach (PackageEntry item in list)
                        {

                            filename = item.GetFullPath().Replace("panorama/images/", ""); //
                            filename = ItemsManager.NameFromAsset(filename);

                            if (filename == itemScreen.name)
                            {
                                if (Desktop)
                                {
                                    string path = modCommon.DataDirectory + "/Loading Screen/";
                                    Paths.EnsureDirectory(path);
                                    string filePath = path + filename + ".png";

                                    Bitmap img = GetTexture(modCommon.package, item, new Rectangle(0, 0, Convert.ToInt32(width), Convert.ToInt32(height)));
                                    img.Save(filePath, ImageFormat.Png);
                                    Wallpaper.Set(filePath, Wallpaper.Style.Stretched);
                                }
                                else if (open)
                                {
                                    string path = modCommon.DataDirectory + "/Loading Screen/";
                                    Paths.EnsureDirectory(path);
                                    string filePath = path + filename + ".png";
                                    if (!File.Exists(filePath))
                                    {
                                        Bitmap img = GetTexture(modCommon.package, item, new Rectangle(0, 0, Convert.ToInt32(width), Convert.ToInt32(height)));
                                        img.Save(filePath, ImageFormat.Png);
                                        Process.Start(filePath);
                                    }
                                    else
                                        Process.Start(filePath);
                                }
                                else
                                {
                                    SaveFileDialog fileDialog = new SaveFileDialog()
                                    {
                                        FileName = filename + ".png",
                                        Title = "Save loading screen file",
                                        Filter = "PNG file | *.png",

                                    };
                                    DialogResult result = fileDialog.ShowDialog();
                                    if (result == DialogResult.OK)
                                    {
                                        var img = GetTexture(modCommon.package, item, new Rectangle(0, 0, Convert.ToInt32(width), Convert.ToInt32(height)));
                                        img.Save(fileDialog.FileName, ImageFormat.Png);
                                    }
                                }

                            }
                        }
                    }
                }
                finally
                {
                    ((IDisposable)modCommon.package)?.Dispose();
                }
            }
            catch (Exception ex)
            {

                return;
            }
        }
        else
        {
            
        }
    }
    public static Bitmap GetTexture(Package apk, PackageEntry entry, Rectangle? resizeTo = null)
    {
        apk.ReadEntry(entry, out byte[] output);
        Resource resource = new Resource();
        resource.Read(new MemoryStream(output));
        ValveResourceFormat.ResourceTypes.Texture texture = (ValveResourceFormat.ResourceTypes.Texture)resource.Blocks[BlockType.DATA];
        Bitmap bmp = texture.GenerateBitmap().ToBitmap();
        Bitmap bmp2;

        if (resizeTo.HasValue)
        {
            bmp2 = bmp;
            bmp = ResizeImage(bmp2, resizeTo.Value.Width, resizeTo.Value.Height);
            bmp2.Dispose();
        }

        return bmp;
    }

    private static Bitmap ResizeImage(Image image, int width, int height)
    {
        var destRect = new Rectangle(0, 0, width, height);
        var destImage = new Bitmap(width, height);

        destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

        using (var graphics = Graphics.FromImage(destImage))
        {
            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            using (var wrapMode = new ImageAttributes())
            {
                wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
            }
        }

        return destImage;
    }

}

