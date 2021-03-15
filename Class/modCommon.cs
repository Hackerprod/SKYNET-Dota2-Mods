using Microsoft.Win32;
using SkynetDota2Mods;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using SkynetDota2Mods.Properties;
using XNova_Utils;
using System.Text;
using System.Drawing.Imaging;
using SKYNET;

public class modCommon
{
    public static string VPKLocation { get; set; }
    public static bool NeedUpdate = false;

    public static string DataDirectory = GetDataDirectory();
    public static string TempDirectory = DataDirectory + "/TEMP";

    public static string DetectedDota2Path = Paths.GetParent(Paths.GetParent(Win32Path())) +  "dota";
    private static Process currentProcess;
    public static Package package;

    public static string GetClientVersion()
    {
        string filepath = Settings.Dota2Path + @"\steam.inf";
        TextBox textBox = new TextBox();

        string settingResult = "";
        try
        {
            byte[] file = File.ReadAllBytes(filepath);
            textBox.Text = Encoding.Default.GetString(file);
            for (int i = 0; i < textBox.Lines.Length; i++)
            {
                if (textBox.Lines[i].StartsWith("ClientVersion"))
                {
                    string[] param = textBox.Lines[i].Split('=');

                    if (param[1].StartsWith(" "))
                        settingResult = param[1].Remove(0, 1);
                    else
                        settingResult = param[1];
                }
            }
        }
        catch { }
        return settingResult;
    }

    public enum Language { Spanish, English }

    public static Language CurrentLanguage = Language.Spanish;
    public static Items ItemIDSelected { get; set; }
    public static bool ExtractFiles { get; set; }
    public static string CurrentDirectory
    {
        get
        {
            Process currentProcess = Process.GetCurrentProcess();
            return new FileInfo(currentProcess.MainModule.FileName).Directory.FullName;
        }
    }
    public static string VPKGeneratorLocation = DataDirectory + "/VPKGenerator";

    public static Image DefaultIcon = Resources.testitem_slot_empty;

    private static ILog log;
    static modCommon()
    {
        log = new ILog();
    }
    private static string GetDataDirectory()
    {
        try
        {
            currentProcess = Process.GetCurrentProcess();
            return new FileInfo(currentProcess.MainModule.FileName).Directory?.FullName + @"\Data";
        }
        finally { currentProcess = null; }
    }
    public static string Win32Path()
    {
        string correctGameFolder = "";
        RegistryKey exeKey = Registry.ClassesRoot.OpenSubKey("dota2\\Shell\\Open\\Command", true);
        if (exeKey != null)
        {
            string exeFile = exeKey.GetValue(null).ToString().Split('"')[1];
            correctGameFolder = Directory.GetParent(exeFile).ToString();

        }
        return correctGameFolder;
    }

    internal static string FirstUpper(string text)
    {
        string result = "";
        for (int i = 0; i < text.Count(); i++)
        {
            if (i == 0)
                result += text[i].ToString().ToUpper();
            else
                result += text[i].ToString().ToLower();
        }
        return result;
    }

    internal static string PreparePrefab(string prefab)
    {
        string result = prefab;
        switch (prefab)
        {
            case "default_item": result = "Default Item"; break;
            case "multikill_banner": result = "multikill banner"; break;
            case "radiantcreeps": result = "creeps"; break;
            case "direcreeps": result = "creeps"; break;
            case "radianttowers": result = "towers"; break;
            case "diretowers": result = "towers"; break;
            case "versus_screen": result = "versus screen"; break;
            case "cursor_pack": result = "cursors"; break;
            case "emoticon_tool": result = "emoticons"; break;
            case "hud_skin": result = "hud skin"; break;
            case "loading_screen": result = "loading"; break;
            default: break;
        }
        return FirstUpper(result);
    }


    public static void WriteLine(string v, bool loading  = false)
    {
        if (loading)
        {
            frmMain.frm.MostrarLabel(v);
        }
    }

    public static void Write(string v)
    {
        WriteLine(v);
    }

    internal static bool GetBool(string boolean)
    {
        switch (boolean.ToLower())
        {
            case "1":
                return true;
            case "true":
                return true;
            case "0":
                return false;
            case "false":
                return false;
            default:
                return false;
        }
    }

    internal static void Show(object v = null)
    {
        if (v == null)
        {
            frmMessage frm = new frmMessage("La variable enviada para mostrar el mensaje es nulo");
            frm.ShowDialog();
        }
        else
        {
            frmMessage frm = new frmMessage(v.ToString());
            frm.ShowDialog();
        }
    }

    internal static void Save(Exception ex)
    {
        ILog.Save(ex);
    }
    internal static void Save(string message, Exception ex)
    {
        ILog.Save(message, ex);
    }

    public static SizeF GetTextSize(string text, Font font)
    {
        using (Graphics graphics = Graphics.FromImage((Image)new Bitmap(1, 1)))
            return graphics.MeasureString(text, font);
    }

    public static string GetVideoFileName(string HeroName)
    {
        string FileName = HeroName + ".webm";
        return FileName.Replace(" ", "_");
    }

    internal static Color ColorItemsFromRarity(string ItemId)
    {
        Color color = default(Color);
        Colors colors;

        Items item = ItemsManager.GetItem(ItemId);

        if (string.IsNullOrEmpty(item.item_rarity))
            colors = frmMain.frm.Colors.Find(x => x.Name.Replace("desc_", "") == "common");
        else
            colors = frmMain.frm.Colors.Find(x => x.Name.Replace("desc_", "") == item.item_rarity);

        color = ColorTranslator.FromHtml(colors.hex_color);

        return color;
    }

    internal static List<string> ReadAllLines(string FilePath)
    {
        List<string> result = new List<string>();
        var file = File.ReadAllBytes(FilePath);
        string text = Encoding.Default.GetString(file);
        TextBox structure = new TextBox();
        structure.Text = text;
        for (int i = 0; i < structure.Lines.Count(); i++)
        {
            result.Add(structure.Lines[i]);
        }
        return result;
    }
    internal static void WriteAllLines(string FilePath, List<string> line)
    {
        string content = "";
        for (int i = 0; i < line.Count; i++)
        {
            if (i == line.Count - 1)
            {
                content += line[i];
            }
            else
                content += line[i] + Environment.NewLine;
        }
        var file = Encoding.Default.GetBytes(content);
        FileStream stream = new FileStream(FilePath, FileMode.OpenOrCreate);
        stream.Write(file, 0, file.Length);
        stream.Close();
    }
    internal static Color ColorBundleFromRarity(string ItemId)
    {
        Color color = default(Color);
        Colors colors;

        Items item = ItemsManager.GetItem(ItemId);

        if (item == null)
            return color;

        if (string.IsNullOrEmpty(item.item_rarity))
            colors = frmMain.frm.Colors.Find(x => x.Name.Replace("desc_", "") == "common");
        else
            colors = frmMain.frm.Colors.Find(x => x.Name.Replace("desc_", "") == item.item_rarity);

        color = ColorTranslator.FromHtml(colors.hex_color);

        return color;
    }
    internal static Color ColorFromRarity(string rarity)
    {
        Color color = default(Color);
        Colors colors;
        try
        {
            if (string.IsNullOrEmpty(rarity))
                colors = frmMain.frm.Colors.Find(x => x.Name.Replace("desc_", "") == "common");
            else
                colors = frmMain.frm.Colors.Find(x => x.Name.Replace("desc_", "") == rarity);

            color = ColorTranslator.FromHtml(colors.hex_color);
        }
        catch (Exception)
        {
            return color;
        }

        return color;
    }
    internal static string GetSlotFromPrefabs(string slot, string type)
    {
        if (!string.IsNullOrEmpty(slot))
            return slot;
        else
        {
            prefabs prefabs = frmMain.frm.prefabs.Find(x => x.Name == type);
            return prefabs.item_slot;
        }
    }

    internal static string GetRarityFromPrefabs(string rarity, string type)
    {
        if (!string.IsNullOrEmpty(rarity))
            return rarity;
        else
        {
            prefabs prefabs = frmMain.frm.prefabs.Find(x => x.Name == type);
            return prefabs.item_rarity;
        }
    }
    public static string ConvertHtmlTotext(string source)
    {
        try
        {
            string result;

            // Remove HTML Development formatting
            // Replace line breaks with space
            // because browsers inserts space
            result = source.Replace("\r", " ");
            // Replace line breaks with space
            // because browsers inserts space
            result = result.Replace("\n", " ");
            // Remove step-formatting
            result = result.Replace("\t", string.Empty);
            // Remove repeating spaces because browsers ignore them
            result = System.Text.RegularExpressions.Regex.Replace(result,
                                                                  @"( )+", " ");

            // Remove the header (prepare first by clearing attributes)
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"<( )*head([^>])*>", "<head>",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"(<( )*(/)( )*head( )*>)", "</head>",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     "(<head>).*(</head>)", string.Empty,
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // remove all scripts (prepare first by clearing attributes)
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"<( )*script([^>])*>", "<script>",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"(<( )*(/)( )*script( )*>)", "</script>",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            //result = System.Text.RegularExpressions.Regex.Replace(result,
            //         @"(<script>)([^(<script>\.</script>)])*(</script>)",
            //         string.Empty,
            //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"(<script>).*(</script>)", string.Empty,
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // remove all styles (prepare first by clearing attributes)
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"<( )*style([^>])*>", "<style>",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"(<( )*(/)( )*style( )*>)", "</style>",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     "(<style>).*(</style>)", string.Empty,
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // insert tabs in spaces of <td> tags
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"<( )*td([^>])*>", "\t",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // insert line breaks in places of <BR> and <LI> tags
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"<( )*br( )*>", "\r",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"<( )*li( )*>", "\r",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // insert line paragraphs (double line breaks) in place
            // if <P>, <DIV> and <TR> tags
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"<( )*div([^>])*>", "\r\r",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"<( )*tr([^>])*>", "\r\r",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"<( )*p([^>])*>", "\r\r",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // Remove remaining tags like <a>, links, images,
            // comments etc - anything that's enclosed inside < >
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"<[^>]*>", string.Empty,
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // replace special characters:
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @" ", " ",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"&bull;", " * ",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"&lsaquo;", "<",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"&rsaquo;", ">",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"&trade;", "(tm)",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"&frasl;", "/",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"&lt;", "<",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"&gt;", ">",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"&copy;", "(c)",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"&reg;", "(r)",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            // Remove all others. More can be added, see
            // http://hotwired.lycos.com/webmonkey/reference/special_characters/
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     @"&(.{2,6});", string.Empty,
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // for testing
            //System.Text.RegularExpressions.Regex.Replace(result,
            //       this.txtRegex.Text,string.Empty,
            //       System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // make line breaking consistent
            result = result.Replace("\n", "\r");

            // Remove extra line breaks and tabs:
            // replace over 2 breaks with 2 and over 4 tabs with 4.
            // Prepare first to remove any whitespaces in between
            // the escaped characters and remove redundant tabs in between line breaks
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     "(\r)( )+(\r)", "\r\r",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     "(\t)( )+(\t)", "\t\t",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     "(\t)( )+(\r)", "\t\r",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     "(\r)( )+(\t)", "\r\t",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            // Remove redundant tabs
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     "(\r)(\t)+(\r)", "\r\r",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            // Remove multiple tabs following a line break with just one tab
            result = System.Text.RegularExpressions.Regex.Replace(result,
                     "(\r)(\t)+", "\r\t",
                     System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            // Initial replacement target string for line breaks
            string breaks = "\r\r\r";
            // Initial replacement target string for tabs
            string tabs = "\t\t\t\t\t";
            for (int index = 0; index < result.Length; index++)
            {
                result = result.Replace(breaks, "\r\r");
                result = result.Replace(tabs, "\t\t\t\t");
                breaks = breaks + "\r";
                tabs = tabs + "\t";
            }

            // That's it.
            return result;
        }
        catch
        {
            MessageBox.Show("Error");
            return source;
        }

    }
    internal static Image GetHeroImage(string name)
    {
        if (File.Exists(DataDirectory + "/panorama/images/heroes/selection/" + name + ".png"))
        {
            return Image.FromFile(DataDirectory + "/panorama/images/heroes/selection/" + name + ".png");
        }
        else
            return Resources.default_item;
    }
    public static Bitmap ChangeOpacity(Image img, float opacityvalue)
    {
        Bitmap bitmap4 = default(Bitmap);
        try
        {
            Bitmap bitmap = new Bitmap(img.Width, img.Height);
            Graphics graphics = Graphics.FromImage(bitmap);
            ColorMatrix newColorMatrix = new ColorMatrix
            {
                Matrix33 = opacityvalue
            };
            ImageAttributes imageAttributes = new ImageAttributes();
            imageAttributes.SetColorMatrix(newColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            graphics.DrawImage(img, new Rectangle(0, 0, bitmap.Width, bitmap.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imageAttributes);
            graphics.Dispose();
            bitmap4 = bitmap;
            return bitmap4;
        }
        catch (Exception ex)
        {
            Exception ex2 = ex;
            return bitmap4;
        }
    }
}
