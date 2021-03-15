using Microsoft.VisualBasic.CompilerServices;
using SkiaSharp.Views.Desktop;
using SkynetDota2Mods.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using ValveResourceFormat;
using ValveResourceFormat.ResourceTypes;

internal class VPKManager
{
    public static Bitmap ExtractAndGenerate(string filePath)
    {
        try
        {
            filePath = "panorama/images/" + filePath + "_png.vtex_c";
            string text = modCommon.VPKLocation;

            using (VpkFile vpkFile = new VpkFile(text))
            {
                vpkFile.Open();
                VpkNode file = vpkFile.GetFile(filePath);
                if (file == null)
                {
                    VpkNode[] allFilesInDirectoryAndSubdirectories = vpkFile.GetAllFilesInDirectoryAndSubdirectories(filePath);
                    if (allFilesInDirectoryAndSubdirectories.Count() == 0)
                    {
                        //modCommon.Show("Entry not found: " + filePath);
                    }
                    else
                    {
                        VpkNode[] array = allFilesInDirectoryAndSubdirectories;
                        foreach (VpkNode node in array)
                        {
                            return ExtractBitmap(text, node);
                        }
                    }
                }
                else
                {
                    return ExtractBitmap(text, file);
                }
                return Resources.default_item;
            }
        }
        catch (Exception)
        {
            return Resources.default_item;
        }
    }

    private static Bitmap ExtractBitmap(string vpkDirFileName, VpkNode node)
    {
        string filename = Path.Combine("data", node.FilePath.Replace("_png.vtex_c", ".png"));
        EnsureDirectoryExists(Path.GetDirectoryName(filename));

        Resource val2 = new Resource();
        using (Stream memoryStream = GetInputStream(vpkDirFileName, node))
        {
            val2.Read(memoryStream);
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
                        bitmap2.Save(filename, ImageFormat.Png);
                        return bitmap2;
                    }
                }
                else
                {
                    bitmap.Save(filename, ImageFormat.Png);
                    return bitmap;
                }
            }

        }
    }
    private static void DoExtractFile(string vpkDirFileName, VpkNode node)
    {
        if ((long)node.ArchiveIndex == 32767)
        {
            modCommon.Show("Found entry: " + node.FilePath);
        }
        else
        {
            modCommon.Show("Found entry: " + node.FilePath + " in VPK " + node.ArchiveIndex);
        }
        ExtractFile(vpkDirFileName, node);
    }
    private static void ExtractFile(string vpkDirFileName, VpkNode node)
    {
        using (Stream stream = GetInputStream(vpkDirFileName, node))
        {
            string[] array = node.FilePath.Split('/');
            IEnumerable<string> source = array.Take(array.Count() - 1);
            string text = array.Last();
            EnsureDirectoryExists(Path.Combine(source.ToArray()));
            using (FileStream fileStream = File.OpenWrite(Path.Combine(array)))
            {
                byte[] array2 = new byte[1024];
                int num = (int)node.EntryLength;
                int num2;
                while ((num2 = stream.Read(array2, 0, array2.Length)) > 0 && num > 0)
                {
                    fileStream.Write(array2, 0, Math.Min(num, num2));
                    num -= num2;
                }
            }
        }
    }

    private static void EnsureDirectoryExists(string directory)
    {
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    private static Stream GetInputStream(string vpkDirFileName, VpkNode node)
    {
        if (node.EntryLength == 0 && node.PreloadBytes > 0)
        {
            return new MemoryStream(node.PreloadData);
        }
        if (node.PreloadBytes != 0)
        {
            throw new NotSupportedException("Unable to get entry data: Both EntryLength and PreloadBytes specified.");
        }
        string text = new string(Enumerable.Repeat('0', 3 - node.ArchiveIndex.ToString().Length).ToArray());
        string path = vpkDirFileName.Replace("_dir.vpk", "_" + text + node.ArchiveIndex + ".vpk");
        FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        fileStream.Seek(node.EntryOffset, SeekOrigin.Begin);
        return fileStream;
    }
    public static void Vpk_ExtractFile(string Vpk_Location, string FileInVPK, string Destination_FileName)
    {
        Package package = new Package();
        package.Read(Vpk_Location);
        package.ReadEntry(package.FindEntry(FileInVPK), out byte[] output);


        FileStream stream = new FileStream(Destination_FileName, FileMode.OpenOrCreate);
        stream.Write(output, 0, output.Length);
        stream.Close();

        //File.WriteAllBytes(Destination_FileName, output);
        package.Dispose();
        GC.Collect();
    }

    public static void ExtractScripts()
    {
        try
        {
            string directory = modCommon.DataDirectory + "/db";
            Paths.EnsureDirectory(directory);
            Vpk_ExtractFile(modCommon.VPKLocation, "scripts/items/items_game.txt", directory + "/items_game.txt");
            Vpk_ExtractFile(modCommon.VPKLocation, "scripts/npc/portraits.txt", directory + "/portraits.txt");

        }
        catch (Exception ex)
        {
            ProjectData.SetProjectError(ex);
            Exception ex2 = ex;
            ProjectData.ClearProjectError();
        }
    }
    public static void ExtractThisLoadingScreen(string filePath)
    {
        try
        {
            filePath = "panorama/images/" + filePath + "_png.vtex_c";
            string text = modCommon.VPKLocation;

            using (VpkFile vpkFile = new VpkFile(text))
            {
                vpkFile.Open();
                VpkNode file = vpkFile.GetFile(filePath);
                if (file == null)
                {
                    
                    VpkNode[] allFilesInDirectoryAndSubdirectories = vpkFile.GetAllFilesInDirectoryAndSubdirectories(filePath);
                    if (allFilesInDirectoryAndSubdirectories.Count() == 0)
                    {
                        modCommon.Show("Load screen not found: " + filePath);
                    }
                    else
                    {
                        VpkNode[] array = allFilesInDirectoryAndSubdirectories;
                        foreach (VpkNode node in array)
                        {

                            ExtractLoadScreen(text, node);
                        }
                    }
                }
            }
        }
        catch (Exception)
        {


        }

    }
    private static void ExtractLoadScreen(string vpkDirFileName, VpkNode node)
    {
        string filename = Path.Combine("data", node.FilePath.Replace("_png.vtex_c", ".png"));
        EnsureDirectoryExists(Path.GetDirectoryName(filename));

        Resource val2 = new Resource();
        using (Stream memoryStream = GetInputStream(vpkDirFileName, node))
        {
            val2.Read(memoryStream);
            using (Bitmap bitmap = ((Texture)val2.Blocks[BlockType.DATA]).GenerateBitmap().ToBitmap())
            {
                using (Bitmap bitmap2 = new Bitmap(1360, 768))
                {
                    using (Graphics graphics = Graphics.FromImage(bitmap2))
                    {
                        graphics.DrawImage(bitmap, new Rectangle(0, 0, bitmap2.Width, bitmap2.Height), new Rectangle(0, 0, bitmap2.Width, bitmap2.Height), GraphicsUnit.Pixel);
                        graphics.Save();
                    }
                    modCommon.Show(filename);
                    bitmap2.Save(filename, ImageFormat.Png);
                }

            }

        }
    }

}

