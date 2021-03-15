using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

class _InIsettings
{
    public static string FilePath = modCommon.DataDirectory + "/Settings.ini";
    static TextBox textBox = new TextBox();
    public static string GetSetting(string settingName)
    {
        if (!File.Exists(FilePath))
        {
            modCommon.Show("File " + FilePath + " not found");
            return "";
        }

        string settingResult = "";
        try
        {
            byte[] file = File.ReadAllBytes(FilePath);
            textBox.Text = Encoding.Default.GetString(file);
            for (int i = 0; i < textBox.Lines.Length; i++)
            {
                if (textBox.Lines[i].StartsWith(settingName))
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
    public static string GetDotaVersion(string settingName)
    {
        string filepath = modCommon.Dota2Path + @"\steam.inf";

        string settingResult = "";
        try
        {
            byte[] file = File.ReadAllBytes(filepath);
            textBox.Text = Encoding.Default.GetString(file);
            for (int i = 0; i < textBox.Lines.Length; i++)
            {
                if (textBox.Lines[i].StartsWith(settingName))
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

    internal static void SetSetting(string settingName, string settingValue)
    {
        bool found = false;

        if (Paths.EnsureSettingsFile(FilePath))
        {
            byte[] file = File.ReadAllBytes(FilePath);
            textBox.Text = Encoding.Default.GetString(file);
            string result = "";
            List<string> lines = new List<string>();
            List<string> SaveList = new List<string>();

            for (int i = 0; i < textBox.Lines.Length; i++)
                lines.Add(textBox.Lines[i]);

            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].StartsWith(settingName))
                {
                    lines[i] = settingName + "=" + settingValue;
                    found = true;
                }

                if (i == lines.Count - 1)
                    result += lines[i];
                else
                    result += lines[i] + Environment.NewLine;

                if (lines[i].Length > 0)
                {
                    SaveList.Add(lines[i]);
                }
            }

            if (!found)
                result += Environment.NewLine + settingName + "=" + settingValue;

            byte[] bytefile = Encoding.Default.GetBytes(result);
            File.WriteAllBytes(FilePath, bytefile);
        }
    }
}

