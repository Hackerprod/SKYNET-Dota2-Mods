using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SKYNET
{
    public class Settings 
    {
        private static GlobalSettings instance;
        public static string Dota2Path { get { return instance.Dota2Path; } set { instance.Dota2Path = value; } }
        public static bool GenerateOnStart { get { return instance.GenerateOnStart; } set { instance.GenerateOnStart = value; } }
        public static bool OpenDota { get { return instance.OpenDota; } set { instance.OpenDota = value; } }
        public static bool ActiveSounds { get { return instance.ActiveSounds; } set { instance.ActiveSounds = value; } }
        public static string ClientVersion { get { return instance.ClientVersion; } set { instance.ClientVersion = value; } }

        static Settings()
        {
            instance = new GlobalSettings();
        }
        public static void Save()
        {
            string FileName = modCommon.CurrentDirectory + "/Data/Settings.ini";
            string json = new JavaScriptSerializer().Serialize(instance);

            EnsureFile(FileName);
            File.WriteAllText(FileName, json);
        }

        private static void EnsureFile(string fileName)
        {
            string patch = Path.GetDirectoryName(fileName);

            if (!Directory.Exists(patch))
                Directory.CreateDirectory(patch);

            if (!File.Exists(fileName))
                File.WriteAllText(fileName, "");
        }

        public static void Load()
        {
            try
            {
                string FileName = modCommon.CurrentDirectory + "/Data/Settings.ini";
                string json = File.ReadAllText(FileName);
                instance = new JavaScriptSerializer().Deserialize<GlobalSettings>(json);

                Dota2Path = instance.Dota2Path;
                GenerateOnStart = instance.GenerateOnStart;
                OpenDota = instance.OpenDota;
                ActiveSounds = instance.ActiveSounds;
                ClientVersion = instance.ClientVersion;
            }
            catch (Exception)
            {
                modCommon.Show("Error reading settings file. Please erase it.");
                Process.GetCurrentProcess().Kill();
            }
        }
    }
    public class GlobalSettings
    {
        public string Dota2Path { get; set; }
        public bool GenerateOnStart { get; set; }
        public bool OpenDota { get; set; }
        public bool ActiveSounds { get; set; }
        public string ClientVersion { get; set; }
    }
}