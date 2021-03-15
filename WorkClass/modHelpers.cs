using SKYNET;
using SkynetDota2Mods;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static SkynetDota2Mods.frmMain;

class modHelpers
{
    public static BackgroundWorker modWorker = new BackgroundWorker();

    public static void ModWorker_DoWork(object sender, DoWorkEventArgs e)
    {
        SelectTab("tabPage6");

        try { Process.GetProcessesByName("dota2")[0].Kill(); } catch { }

        modCommon.WriteLine("Deleting Last Mods", true);
        FileManager.DeleteDirectory(modCommon.VPKGeneratorLocation + "/pak01_dir/");
        FileManager.DeleteDirectory(Paths.GetParent(Settings.Dota2Path).ToString() + "/SkynetMod");
        string pak01_dir = Path.Combine(modCommon.VPKGeneratorLocation, "pak01_dir");
        Paths.CleanDirectory(pak01_dir);

        modCommon.WriteLine("Loading Items Database", true);
        KValue kValue = DotaResources.kValue.Clone();

        modCommon.WriteLine("Preparing external mods", true);
        string ExternalItems = Path.Combine(modCommon.DataDirectory, "ExternalItems");
        string[] externals = Directory.GetFiles(ExternalItems, "*", SearchOption.AllDirectories);
        foreach (string external in externals)
        {
            string targetfile = modCommon.VPKGeneratorLocation + "/pak01_dir" + external.Replace(ExternalItems, "");

            if (!File.Exists(targetfile))
            {
                try
                {
                    Paths.EnsureDirectory(Path.GetDirectoryName(targetfile));
                    File.Copy(external, targetfile, true);
                } catch { }
            }
            
        }
        
        

        //RadiantTowers
        modCommon.WriteLine("Configuring Radiant Towers", true);
        if (!string.IsNullOrEmpty(manager.RadiantTowers?.ItemID))
        {
            Items item = frm.DefaultItems.Find(x => x.prefab == "radianttowers" && x.name.StartsWith("Default "));
            SetItem(kValue, item?.ItemID, manager.RadiantTowers);
        }
        //DireTowers
        modCommon.WriteLine("Configuring Dire Towers", true);
        if (!string.IsNullOrEmpty(manager.DireTowers?.ItemID))
        {
            Items item = frm.DefaultItems.Find(x => x.prefab == "diretowers" && x.name.StartsWith("Default "));
            SetItem(kValue, item?.ItemID, manager.DireTowers);
        }
        //MusicPack
        modCommon.WriteLine("Configuring Music Pack", true);
        if (!string.IsNullOrEmpty(manager.MusicPack?.ItemID))
        {
            Items item = frm.DefaultItems.Find(x => x.prefab == "music" && x.name.StartsWith("Default "));
            SetItem(kValue, item?.ItemID, manager.MusicPack);
        }
        //Announcer
        modCommon.WriteLine("Configuring Announcer", true);
        if (!string.IsNullOrEmpty(manager.Announcer?.ItemID))
        {
            Items item = frm.DefaultItems.Find(x => x.prefab == "announcer" && x.item_slot == "announcer" && x.name.StartsWith("Default "));
            SetItem(kValue, item?.ItemID, manager.Announcer);
        }
        //Announcer
        modCommon.WriteLine("Configuring Megakill Announcer", true);
        if (!string.IsNullOrEmpty(manager.MegaKillAnnouncer?.ItemID))
        {
            Items item = frm.DefaultItems.Find(x => x.prefab == "announcer" && x.item_slot == "mega_kills" && x.name.StartsWith("Default "));
            SetItem(kValue, item?.ItemID, manager.MegaKillAnnouncer);
        }

        //Terrain
        modCommon.WriteLine("Configuring Terrain", true);
        if (!string.IsNullOrEmpty(manager.Terrain?.ItemID))
        {
            Items item = frm.DefaultItems.Find(x => x.prefab == "terrain" && x.name.StartsWith("Default "));
            SetItem(kValue, item?.ItemID, manager.Terrain);
        }
        //WeatherEffect
        modCommon.WriteLine("Configuring Weather Effect", true);
        if (!string.IsNullOrEmpty(manager.WeatherEffect?.ItemID))
        {
            Items item = frm.DefaultItems.Find(x => x.prefab == "weather" && x.name.StartsWith("Default "));
            SetItem(kValue, item?.ItemID, manager.WeatherEffect);
        }
        //RadiantCreeps
        modCommon.WriteLine("Configuring Radiant Creeps", true);
        if (!string.IsNullOrEmpty(manager.RadiantCreeps?.ItemID))
        {
            Items item = frm.DefaultItems.Find(x => x.prefab == "radiantcreeps" && x.name.StartsWith("Default "));
            SetItem(kValue, item?.ItemID, manager.RadiantCreeps);
        }
        //DireCreeps
        modCommon.WriteLine("Configuring Dire Creeps", true);
        if (!string.IsNullOrEmpty(manager.DireCreeps?.ItemID))
        {
            Items item = frm.DefaultItems.Find(x => x.prefab == "direcreeps" && x.name.StartsWith("Default "));
            SetItem(kValue, item?.ItemID, manager.DireCreeps);
        }
        //CursorPack
        modCommon.WriteLine("Configuring Cursor Pack", true);
        if (!string.IsNullOrEmpty(manager.CursorPack?.ItemID))
        {
            Items item = frm.DefaultItems.Find(x => x.prefab == "cursor_pack" && x.name.StartsWith("Default "));
            SetItem(kValue, item?.ItemID, manager.CursorPack);
        }
        //LoadingScreen
        modCommon.WriteLine("Configuring Loading Screen", true);
        if (!string.IsNullOrEmpty(manager.LoadingScreen?.ItemID))
        {
            Items item = frm.DefaultItems.Find(x => x.prefab == "loading_screen" && x.name.StartsWith("Default "));
            SetItem(kValue, item?.ItemID, manager.LoadingScreen);
        }
        //VersusScreen
        modCommon.WriteLine("Configuring Versus Screen", true);
        if (!string.IsNullOrEmpty(manager.VersusScreen?.ItemID))
        {
            Items item = frm.DefaultItems.Find(x => x.prefab == "versus_screen" && x.name.StartsWith("Default "));
            SetItem(kValue, item?.ItemID, manager.VersusScreen);
        }
        //Emoticons
        modCommon.WriteLine("Configuring Emoticons", true);
        if (!string.IsNullOrEmpty(manager.Emoticons?.ItemID))
        {
            Items item = frm.DefaultItems.Find(x => x.prefab == "emoticon_tool" && x.name.StartsWith("Default "));
            SetItem(kValue, item?.ItemID, manager.Emoticons);
        }
        //Courier
        modCommon.WriteLine("Configuring Courier", true);
        if (!string.IsNullOrEmpty(manager.Courier?.ItemID))
        {
            Items item = frm.DefaultItems.Find(x => x.prefab == "courier" && x.name.StartsWith("Default "));
            SetItem(kValue, item?.ItemID, manager.Courier);
        }
        //Ward
        modCommon.WriteLine("Configuring Ward", true);
        if (!string.IsNullOrEmpty(manager.Ward?.ItemID))
        {
            Items item = frm.DefaultItems.Find(x => x.prefab == "ward" && x.name.StartsWith("Default "));
            SetItem(kValue, item?.ItemID, manager.Ward);
        }
        //HUDSkin
        modCommon.WriteLine("Configuring HUD Skin", true);
        if (!string.IsNullOrEmpty(manager.HUDSkin?.ItemID))
        {
            Items item = frm.DefaultItems.Find(x => x.prefab == "hud_skin" && x.name.StartsWith("Default "));
            SetItem(kValue, item?.ItemID, manager.HUDSkin);
        }
        //MultikillBanner
        modCommon.WriteLine("Configuring Multikill Banner", true);
        if (!string.IsNullOrEmpty(manager.MultikillBanner?.ItemID))
        {
            Items item = frm.DefaultItems.Find(x => x.prefab == "multikill_banner" && x.name.StartsWith("Default "));
            SetItem(kValue, manager.MultikillBanner.ItemID, manager.MultikillBanner);
        }
        //Emblem
        modCommon.WriteLine("Configuring Emblem", true);
        if (!string.IsNullOrEmpty(manager.Emblem?.ItemID))
        {
            string NewId = manager.Emblem.ItemID;
            SetItem(kValue, manager.Emblem.ItemID, manager.Emblem);
        }
        //Pets
        modCommon.WriteLine("Configuring hero pets", true);
        if (!string.IsNullOrEmpty(manager.Pets?.ItemID))
        {
            Items item = frm.DefaultItems.Find(x => x.item_name.Contains("DefaultPet"));
            SetItem(kValue, item?.ItemID, manager.Pets);
        }

        //Active defaults taunt
        modCommon.WriteLine("Configuring Taunts", true);
        Items Dtaunt = frm.DefaultItems.Find(x => x.item_slot == "taunt");
        if (!string.IsNullOrEmpty(Dtaunt?.ItemID))
        {
            KValue Update = new KValue();
            Update.Children = kValue["Items"][Dtaunt?.ItemID].Children;
            Update["prefab"].Value = "wearable";
            kValue["Items"][Dtaunt?.ItemID].Children = Update.Children;
        }



        for (int i = 0; i < manager.Heroes.Count; i++)
        {
            string HeroName = manager.Heroes[i].Name;

            modCommon.WriteLine("PROCESSING " + ItemsManager.GetHeroName(HeroName).ToUpper(), true);
            foreach (Items ActiveItem in manager.Heroes[i].Bundles)
            {
                if (ActiveItem.ItemID != "0000")
                {
                    if (ActiveItem.item_slot == "taunt")
                    {
                        foreach (var taunt in frm.Taunts)
                        {
                            if (taunt.used_by_heroes == HeroName)
                            {
                                if (taunt.ItemID == ActiveItem.ItemID)
                                {
                                    if (kValue["Items"][ActiveItem.ItemID].ContainsKey("baseitem"))
                                        kValue["Items"][ActiveItem.ItemID]["baseitem"].Value = "1";
                                    else
                                        kValue["Items"][ActiveItem.ItemID].Children.Add(new KValue("baseitem", "1"));
                                }
                                else
                                {
                                    if (kValue["Items"][ActiveItem.ItemID].ContainsKey("baseitem"))
                                        kValue["Items"][ActiveItem.ItemID].RemoveKey("baseitem");
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (Items itemdef in frm.DefaultItems)
                        {
                            if (itemdef.used_by_heroes == HeroName && itemdef.item_slot == ActiveItem.item_slot)
                            {
                                if (ActiveItem.ItemID != "0000")
                                {
                                    SetItem(kValue, itemdef.ItemID, ActiveItem);
                                }
                            }
                        }

                    }
                }
            }
        }

        string saveTo = modCommon.VPKGeneratorLocation + "/pak01_dir/scripts/items";
        Paths.EnsureDirectory(saveTo);

        modCommon.WriteLine("Saving Custom Items Database", true);
        kValue.SaveToFile(saveTo + "/items_game.txt");
        Paths.EnsureDirectory(modCommon.VPKGeneratorLocation + "/pak01_dir/scripts/npc");
        FileManager.FileCopy(modCommon.DataDirectory + "/db/portraits.txt", modCommon.VPKGeneratorLocation + "/pak01_dir/scripts/npc/portraits.txt");



        modCommon.WriteLine("Creating vpk file", true);
        Process startInfo = new Process();
        startInfo.StartInfo.Arguments = " pak01_dir";
        startInfo.StartInfo.WorkingDirectory = modCommon.VPKGeneratorLocation;
        startInfo.StartInfo.Verb = "runas";
        startInfo.StartInfo.FileName = modCommon.VPKGeneratorLocation + "/vpk.exe";
        startInfo.StartInfo.CreateNoWindow = true;
        startInfo.StartInfo.UseShellExecute = false;
        startInfo.Start();
        startInfo.WaitForExit();

        string modLocation = Paths.GetParent(Settings.Dota2Path).ToString() + "/SkynetMods";
        Paths.EnsureDirectory(modLocation);

        FileManager.MakeFolderAllUsersAccessible(modCommon.VPKGeneratorLocation);
        FileManager.MakeFolderAllUsersAccessible(modLocation);

        FileManager.FileMove(modCommon.VPKGeneratorLocation + "/pak01_dir.vpk", modLocation + "/pak01_dir.vpk");

        modCommon.WriteLine("Verifing gameinfo", true);
        bool ready = PrepareGameInfo();
        if (!ready)
            modCommon.Show("Error processing gameinfo.gi file");
    }
    private static void SetItem(KValue kValue, string defaultItem, Items currentItem)
    {

        if (currentItem.ItemID == "0000")
            return;

        int Count = 1;

        KValue Update = new KValue();
        Update.Children = kValue["Items"][currentItem.ItemID].Clone().Children;

        //Manage Style
        if (currentItem.style != null && !string.IsNullOrEmpty(currentItem.style.ID))
        {
            if (currentItem.style.ID == "0")
                goto Label_1;

            if (Update.ContainsKey("visuals"))
            {
                List<KValue>.Enumerator visuals = Update["visuals"].Children.GetEnumerator();
                while (visuals.MoveNext())
                {
                    KValue currentvisuals = visuals.Current;

                    //Modify style from asset_modifier
                    if (currentvisuals.Name.StartsWith("asset_modifier"))
                    {
                        if (currentvisuals.ContainsKey("type"))
                        {
                            if (currentvisuals.ContainsKey("style"))
                            {
                                if (currentvisuals["style"].Value.ToString() == "0")
                                {
                                    currentvisuals["style"].Value = currentItem.style.ID;
                                }
                                else if (currentvisuals["style"].Value.ToString() == currentItem.style.ID)
                                {
                                    currentvisuals.RemoveKey("style");
                                }
                            }

                            if (currentvisuals["type"].Value.ToString() == "activity" && currentvisuals.ContainsKey("style"))
                                currentvisuals.RemoveKey("style");
                        }
                    }
                }
            }

            //Remove styles from styles list in "visuals/styles"
            Items item = ItemsManager.GetItem(currentItem.ItemID);
            for (int i = 0; i < item?.styles.Count; i++)
            {
                if (item.styles[i].ID != currentItem.style.ID)
                    Update["visuals"]["styles"].RemoveKey(item.styles[i].ID);
            }

        Label_1:;
        }

        if (!Update.ContainsKey("baseitem") && Count == 1)
        {
            Update.Children.Add(new KValue("baseitem", "1"));
            Count = Count + 1;
        }

        if (Update.ContainsKey("prefab"))
            Update["prefab"].Value = kValue["Items"][defaultItem]["prefab"]?.Value;

        kValue["Items"][defaultItem].Children = Update.Children;



        modCommon.ExtractFiles = false;
        //Save files 
        if (!modCommon.ExtractFiles)
            return;

        string pak01_dir = Path.Combine(modCommon.VPKGeneratorLocation, "pak01_dir");

        Paths.EnsureDirectory(pak01_dir);

        if (Update.ContainsKey("model_player"))
        {
            DotaResources.ExtractFile(Update["model_player"].Value?.ToString(), pak01_dir);
        }
        if (Update.ContainsKey("visuals"))
        {

            List<KValue>.Enumerator visuals = Update["visuals"].Children.GetEnumerator();
            while (visuals.MoveNext())
            {
                KValue currentvisuals = visuals.Current;

                //Extract files from asset_modifier
                if (currentvisuals.Name.StartsWith("asset_modifier"))
                {
                    List<KValue>.Enumerator asset_modifier = currentvisuals.Children.GetEnumerator();
                    while (asset_modifier.MoveNext())
                    {
                        KValue asset = asset_modifier.Current;

                        if (asset.Value != null)
                        {
                            if (asset.Value.ToString().Contains("/"))
                            {
                                string[] slachCount = asset.Value.ToString().Split('/');
                                if (slachCount.Count() > 2)
                                {
                                    DotaResources.ExtractFile(asset.Value.ToString(), pak01_dir);
                                }
                            }
                        }
                    }
                }
            }
            ///
        }
    }

    private static bool PrepareGameInfo()
    {
        ClearGameInfo();

        try
        {
            string gameInfo = Settings.Dota2Path + @"\gameinfo.gi";
            if (File.Exists(gameInfo))
            {
                List<string> line = modCommon.ReadAllLines(gameInfo);
                for (int i = 0; i < line.Count; i++)
                {

                    if (line[i].Contains("Game_Language") && !line.Contains("			Game				" + "SkynetMods"))
                    {
                        line[i] = "			Game				" + "SkynetMods" + Environment.NewLine + line[i];
                    }
                    if (line[i].Contains("Game				dota") && !line.Contains("			Game				" + "SkynetMods"))
                    {
                        line[i] = "			Game				" + "SkynetMods" + Environment.NewLine + line[i];
                    }
                    if (line[i].Contains("Mod					dota") && !line.Contains("			Mod				" + "SkynetMods"))
                    {
                        line[i] = "			Mod				    " + "SkynetMods" + Environment.NewLine + line[i];
                    }
                }
                modCommon.WriteAllLines(gameInfo, line);
            }


            return true;
        }
        catch (Exception ex) { modCommon.Save(ex); return false; }

    }
    public static void ClearGameInfo()
    {
        List<string> ToClear = new List<string>();
        string gameInfo = Settings.Dota2Path + @"\gameinfo.gi";

        KValue kValue = KValue.LoadAsText(gameInfo);

        List<KValue> lista = new List<KValue>();

        if (kValue != null)
        {
            if (kValue.ContainsKey("FileSystem"))
            {
                if (kValue["FileSystem"].ContainsKey("SearchPaths"))
                {
                    List<KValue> list = kValue["FileSystem"]["SearchPaths"].Children;

                    //Clear Mods
                    for (int i = 0; i < list.Count; i++)
                    {
                        switch (list[i].Name)
                        {
                            case "Game":
                                if (list[i].Value.ToString() != "dota" && list[i].Value.ToString() != "core")
                                    if (!ToClear.Contains(list[i].Value.ToString()))
                                        ToClear.Add(list[i].Value.ToString());

                                break;
                        }
                    }
                }
            }
        }

        //Clear Mods
        if (ToClear.Any())
        {
            if (File.Exists(gameInfo))
            {
                List<string> lines = modCommon.ReadAllLines(gameInfo);

                for (int i = 0; i < ToClear.Count; i++)
                {
                    lines.RemoveAll(x => x.ToLower().Contains("game") && x.ToLower().Contains(ToClear[i].ToLower()));
                    lines.RemoveAll(x => x.ToLower().Contains("mod") && x.ToLower().Contains(ToClear[i].ToLower()));
                }

                File.WriteAllLines(gameInfo, lines);
            }
        }
    }

    public static void ModWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        if (Settings.OpenDota)
        {
            try { Process.GetProcessesByName("dota2")[0].Kill(); } catch { }

            try
            {
                modCommon.WriteLine("Opening Dota2", true);
                string dota2ExE = Path.Combine(Paths.GetParent(Settings.Dota2Path), "bin", "win32") + "/dota2.exe";
                Process.Start(dota2ExE);
            }
            catch { }
            Thread.Sleep(1000);
        }
        modCommon.WriteLine("", true);
        SelectTab(CurrentTab);
        AudioPlayer.PlaySound("sounds/ui/bonus_level.vsnd", false);
    }
}
