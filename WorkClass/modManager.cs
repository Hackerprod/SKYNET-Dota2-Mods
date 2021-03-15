using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using static SkynetDota2Mods.frmMain;

namespace SkynetDota2Mods
{
    public class modManager
    {
        public List<Hero> Heroes { get; set; } = new List<Hero>();

        //Sound items
        public Items MusicPack { get; set; }
        public Items Announcer { get; set; }
        public Items MegaKillAnnouncer { get; set; }

        //Terrain items
        public Items Terrain { get; set; }
        public Items WeatherEffect { get; set; }
        public Items RadiantCreeps { get; set; }
        public Items DireCreeps { get; set; }
        public Items RadiantTowers { get; set; }
        public Items DireTowers { get; set; }

        //Single Source
        public Items CursorPack { get; set; }
        public Items MultikillBanner { get; set; }
        public Items Emblem { get; set; }
        public Items LoadingScreen { get; set; }
        public Items VersusScreen { get; set; }
        public Items Emoticons { get; set; }

        //Multiple Styles
        public Items Courier { get; set; }
        public Items Ward { get; set; }
        public Items HUDSkin { get; set; }
        public Items Pets { get; set; }
        public Items StreakEffect { get; set; }

        

        public modManager()
        {
            modHelpers.modWorker = new BackgroundWorker();
            modHelpers.modWorker.DoWork += modHelpers.ModWorker_DoWork;
            modHelpers.modWorker.RunWorkerCompleted += modHelpers.ModWorker_RunWorkerCompleted;
        }



        internal void Save()
        {
            string FileName = modCommon.DataDirectory + "/[SKYNET] Dota2 Mods.bin";
            string json = new JavaScriptSerializer().Serialize(this);
            File.WriteAllText(FileName, json);
        }

        internal void Load()
        {

            modManager modmanager = new modManager();

            try
            {
                string FileName = modCommon.DataDirectory + "/[SKYNET] Dota2 Mods.bin";
                string json = File.ReadAllText(FileName);
                modmanager = new JavaScriptSerializer().Deserialize<modManager>(json);
            }
            catch
            {
                frmMain.manager = new modManager();
            }
            frmMain.manager = modmanager;
        }

        public void CreateMod()
        {
            modHelpers.modWorker.RunWorkerAsync();
        }

        public static void SetHeroesToList()
        {
            KValue kValue = KValue.LoadAsText(Path.Combine("data", "db", "activelist.txt"));
            if (kValue != null)
            {
                List<KValue>.Enumerator enumerator = kValue.Children.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        KValue hero = enumerator.Current;
                        Hero NewHero = manager.Heroes.Find((Hero x) => x.Name == hero.Name);;

                        if (NewHero == null)
                        {
                            frmMain.manager.Heroes.Add(new Hero() { Name = hero.Name });
                        }
                    }
                }
                finally
                {
                    ((IDisposable)enumerator).Dispose();
                }

            }
        }
    }
}
