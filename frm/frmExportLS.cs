using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SkiaSharp.Views.Desktop;
using SkynetDota2Mods;
using ValveResourceFormat;
using ValveResourceFormat.ResourceTypes;

public partial class frmExportLS : Form
{
    private bool mouseDown;     //Mover ventana
    private Point lastLocation; //Mover ventana
    private Items item;

    public frmExportLS()
    {
        InitializeComponent();
        CheckForIllegalCrossThreadCalls = false;  //Para permitir acceso a los subprocesos

    }

    public frmExportLS(Items item)
    {
        InitializeComponent();
        CheckForIllegalCrossThreadCalls = false;  //Para permitir acceso a los subprocesos

        this.item = item;
    }

    public void MostrarLabel(string v)
    {
        if (File.Exists(lastscreen))
        {
            LoadScreen.Image = Image.FromFile(lastscreen);
        }
        txtMessage.Text = v;
    }

    private void Event_MouseMove(object sender, MouseEventArgs e)
    {
        if (mouseDown)
        {
            Location = new Point((Location.X - lastLocation.X) + e.X, (Location.Y - lastLocation.Y) + e.Y);
            Update();
            Opacity = 0.93;
        }
    }

    private void Event_MouseDown(object sender, MouseEventArgs e)
    {
        mouseDown = true;
        lastLocation = e.Location;

    }

    private void Event_MouseUp(object sender, MouseEventArgs e)
    {
        mouseDown = false;
        Opacity = 100;
    }


    private void cancelBtn_Click(object sender, EventArgs e)
    {
        ExportWorker.CancelAsync();
        Close();
    }
    string lastscreen = "";
    private void ExportWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
    {
        Rectangle workingArea1 = Screen.FromHandle(Handle).WorkingArea;
        int width = workingArea1.Width;
        int height = workingArea1.Height;

        string filename = "";
        string text3 = modCommon.DataDirectory + "/Loading Screen";
        Paths.EnsureDirectory(text3);

        string DirectoryName = "panorama/images/loadingscreen";
        if (modCommon.package != null)
        {
            try
            {
                try
                {
                    List<PackageEntry> list = (from l in modCommon.package.Entries["vtex_c"]
                                               where l.DirectoryName.Contains(DirectoryName)
                                               select l).ToList();
                    if (list.Any())
                    {
                        int num = 0;
                        foreach (PackageEntry item in list)
                        {
                            if (ExportWorker.CancellationPending)
                            {
                                e.Cancel = true;
                                return;
                            }
                            filename = item.GetFullPath().Replace("panorama/images/", ""); //

                            filename = ItemsManager.NameFromAsset(filename);

                            string path = Path.Combine(text3, filename + ".png").Replace(" .png", ".png"); ;
                            //modCommon.Show(path);
                            if (!File.Exists(path))
                            {

                                var img = GetTexture(modCommon.package, item, new Rectangle(0, 0, Convert.ToInt32(width), Convert.ToInt32(height)));
                                img.Save(path, ImageFormat.Png);
                                lastscreen = path.Replace(@"\", "/").Replace(@"\\", "/");
                                MostrarLabel("Extracted " + filename);

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
            MostrarLabel("Export finished");
        }
        else
        {
            //modCommon.Show("4");
        }

    }
    public static Bitmap GetTexture(Package apk, PackageEntry entry, Rectangle? resizeTo = null)
    {
        byte[] output = new byte[256];
        apk.ReadEntry(entry, out output);
        var res = new Resource();
        using (var resMem = new MemoryStream(output))
        {
            res.Read(resMem);

            if (res.ResourceType != ResourceType.Texture)
            {
                throw new NotSupportedException();
            }

            var tex = (Texture)res.Blocks[BlockType.DATA];
            Bitmap bmp2;
            var bmp = tex.GenerateBitmap().ToBitmap();

            if (resizeTo.HasValue)
            {
                bmp2 = bmp;
                bmp = ResizeImage(bmp2, resizeTo.Value.Width, resizeTo.Value.Height);
                bmp2.Dispose();
            }

            return bmp;
        }

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

    private void FrmExportLS_Shown(object sender, EventArgs e)
    {
        ExportWorker.RunWorkerAsync();
    }

    private void ExportWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
    {
        if (e.Cancelled)
        {
            //MessageBox.Show("The task has been cancelled");
        }
        else if (e.Error != null)
        {
            //MessageBox.Show("Error. Details: " + (e.Error as Exception).ToString());
        }
        else
        {
            try
            {
                ProcessStartInfo LScreen = new ProcessStartInfo();
                LScreen.FileName = "explorer.exe";
                LScreen.Arguments = modCommon.DataDirectory + "/Loading Screen";
                LScreen.WindowStyle = ProcessWindowStyle.Minimized;
                //Process.Start(LScreen);
            }
            catch { }
        }
    }

}

