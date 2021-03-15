using Dota2;
using Microsoft.Win32;
using Skynet;
using Skynet.CoreServices;
using SkynetDota2Mods.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ValveResourceFormat;
using ValveResourceFormat.ResourceTypes;
using XNova_Utils;
using SKYNET;

namespace SkynetDota2Mods
{
    public partial class frmMain : Form
    {
        public static frmMain frm;
        private bool FirstLaunch = false;
        private bool mouseDown;     //Mover ventana
        private Point lastLocation; //Mover ventana
        private string SelectedMenu = "HEROES";
        Media media;
        private Keyboard hook;
        string VideoFolder;
        public static modManager manager;

        public List<Items> items = new List<Items>();
        public List<Items> DefaultItems = new List<Items>();
        public List<Items> Bundles = new List<Items>();
        public List<Items> Towers = new List<Items>();
        public List<Items> Taunts = new List<Items>();

        //Sound items
        public List<Items> MusicPack = new List<Items>();
        public List<Items> Announcer = new List<Items>();
        public List<Items> MegaKillAnnouncer = new List<Items>();
        public List<Items> Pets = new List<Items>();
        
        //Terrain items
        public List<Items> Terrain = new List<Items>();
        public List<Items> WeatherEffect = new List<Items>();
        public List<Items> RadiantCreeps = new List<Items>();
        public List<Items> DireCreeps = new List<Items>();
        public List<Items> RadiantTowers = new List<Items>();
        public List<Items> DireTowers = new List<Items>();



        //Single Source
        public List<Items> CursorPack = new List<Items>();
        public List<Items> MultikillBanner = new List<Items>();
        public List<Items> Emblem = new List<Items>();
        public List<Items> LoadingScreen = new List<Items>();
        public List<Items> VersusScreen = new List<Items>();
        public List<Items> Emoticons = new List<Items>();

        //Multiple Styles
        public List<Items> Courier = new List<Items>();
        public List<Items> Ward = new List<Items>();
        public List<Items> HUDSkin = new List<Items>();
        public List<ItemDescription> itemDescription = new List<ItemDescription>();
        public List<ItemDescription> rich_presence = new List<ItemDescription>();

        public List<Items> StreakEffect = new List<Items>();

        
        //For mi
        public List<Colors> Colors = new List<Colors>();
        public List<prefabs> prefabs = new List<prefabs>();
        private bool tabPage3Loaded;
        private bool tabPage4Loaded;

        public bool LoadVideo { get; }
        public bool Updating { get; private set; }
        public bool Started { get; private set; }
        public static string CurrentTab { get; set; }
        public bool Ready { get; private set; }

        List<Control> Menus = new List<Control>();

        public frmMain()
        {
            InitializeComponent();
            AutoScaleMode = AutoScaleMode.Inherit;

            CheckForIllegalCrossThreadCalls = false;
            test();
            frm = this;
            this.tabControl1.Size = new System.Drawing.Size(1199, 486);
            SetParents();

            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            if (currentCulture.ToString().Contains("es"))
            {
                modCommon.CurrentLanguage = modCommon.Language.Spanish;
            }
            else
                modCommon.CurrentLanguage = modCommon.Language.English;

            if (!Directory.Exists(modCommon.DataDirectory))
                FirstLaunch = true;

            if (!File.Exists(modCommon.DataDirectory + "/Settings.ini"))
                FirstLaunch = true;

            manager = new modManager();
            manager.Load();

            SelectTab("tabPage6");

            if (FirstLaunch)
            {

                if (Directory.Exists(modCommon.DetectedDota2Path))
                {
                    Settings.Dota2Path = modCommon.DetectedDota2Path;
                    modCommon.VPKLocation = Settings.Dota2Path + "/pak01_dir.vpk";
                }
                else
                {
                    var openDialog = new OpenFileDialog
                    {
                        Title = "Select pak01_dir.vpk in \"" + @"dota 2 beta\game\dota" + "\"",
                        Filter = "Valve File | pak01_dir.vpk",
                    };
                    DialogResult userOK = openDialog.ShowDialog();

                    if (userOK == DialogResult.OK)
                    {
                        Settings.Dota2Path = Path.GetDirectoryName(openDialog.FileName);
                        modCommon.VPKLocation = openDialog.FileName;
                    }
                }

                Settings.GenerateOnStart = true;
                Settings.OpenDota = false;
                Settings.ActiveSounds = true;
                Settings.ClientVersion = modCommon.GetClientVersion();
                Settings.Save();
            }
            else
            {

                Settings.Load();

                modCommon.VPKLocation = Settings.Dota2Path + "/pak01_dir.vpk";

                string version = "0";
                if (File.Exists(Settings.Dota2Path + @"\steam.inf"))
                    version = Settings.ClientVersion;

                if (version != "0")
                {
                    if (modCommon.GetClientVersion() != version)
                    {
                        FileManager.DeleteDirectory(Path.Combine(modCommon.DataDirectory, "db"));
                        modCommon.NeedUpdate = true;
                        Settings.ClientVersion = version;
                    }
                }
            }

            if (!File.Exists(modCommon.VPKLocation))
            {
                modCommon.Show("File " + modCommon.VPKLocation + " not found");
                modCommon.Show(Settings.Dota2Path);

                if (Directory.Exists(modCommon.DetectedDota2Path))
                {
                    Settings.Dota2Path = modCommon.DetectedDota2Path;
                    modCommon.VPKLocation = Settings.Dota2Path + "/pak01_dir.vpk";
                }
                else
                {
                    var openDialog = new OpenFileDialog
                    {
                        Title = "Select pak01_dir.vpk in \"" + @"dota 2 beta\game\dota" + "\"",
                        Filter = "Valve File | pak01_dir.vpk",
                    };
                    DialogResult userOK = openDialog.ShowDialog();

                    if (userOK == DialogResult.OK)
                    {
                        Settings.Dota2Path = Path.GetDirectoryName(openDialog.FileName);
                        modCommon.VPKLocation = openDialog.FileName;
                    }
                }

                if (!File.Exists(modCommon.VPKLocation))
                {
                    hook.Remove();
                    Process.GetCurrentProcess().Kill();
                }
            }

            Paths.EnsureDirectory(modCommon.DataDirectory + "/ExternalItems");

            if (Directory.Exists(Settings.Dota2Path))
            {
                VideoFolder = Settings.Dota2Path + @"\panorama\videos\heroes";

                if (Directory.Exists(VideoFolder))
                    if (Directory.GetFiles(VideoFolder).Length > 1)
                        LoadVideo = true;
            }

            CursorWorker.RunWorkerAsync();
            ResourcesWorker.RunWorkerAsync();
            media = new Media();

            ClientVersion.Text = "Client version: " + Settings.ClientVersion;

            hook = new Keyboard();
            hook.Install(Handle);

        }

        private void test()
        {
            //IniClass.SetIniSectionKey("SkynetDota2Mods", "Key", DateTime.Now.ToLongTimeString(), @"D:\Instaladores\Programación\Projects\SkynetDota2Mods\SkynetDota2Mods\bin\x64\Debug\IniClass.ini");
            // modCommon.Show(IniClass.GetIniSectionKey(""));
            //modCommon.Show("");
        }

        private void SetParents()
        {
            Menus.Add(labelHeroes);
            Menus.Add(labelWord);
            Menus.Add(labelMisc);
            Menus.Add(labelCreateMod);
            Menus.Add(panelHeroes);
            Menus.Add(panelWord);
            Menus.Add(panelMisc);
            Menus.Add(panelCreateMod);

            foreach (Control item in Menus)
            {
                if (item is Panel)
                {
                    item.Parent = banner;
                }
                //
            }

            CloseBtn.Parent = banner;
            OpenSettings.Parent = banner;
            OpenDota.Parent = banner;
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists(modCommon.DataDirectory))
            {
                //Directory.CreateDirectory(modCommon.DataDirectory);
                //GenerateHeroes();
            }
            //GenerateHeroes();
        }

        private void ResourcesWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Updating = true;
            SelectTab("tabPage6");

            modCommon.WriteLine("Starting...", true);
            modCommon.package = new Package();
            modCommon.package.Read(modCommon.VPKLocation);


            modCommon.WriteLine("Extracting Items Database", true);
            RegenItemMatrixCommand regenItemMatrix = new RegenItemMatrixCommand();
            regenItemMatrix.ProcessCommand(modCommon.package);
            
            modCommon.WriteLine("Extracting Hero icon images", true);
            DotaResources.ExtractResources("panorama/images/heroes", "Heroes");

            modCommon.WriteLine("Extracting default items images", true);
            DotaResources.ExtractResources("panorama/images/econ/heroes", "Items");

            modCommon.WriteLine("Extracting items images", true);
            DotaResources.ExtractResources("panorama/images/econ/items", "Items");

            modCommon.WriteLine("Extracting pets images", true);
            DotaResources.ExtractResources("panorama/images/econ/pets", "Items");

            modCommon.WriteLine("Extracting sets images", true);
            DotaResources.ExtractResources("panorama/images/econ/sets", "Items");

            modCommon.WriteLine("Extracting Loading Screen", true);
            DotaResources.ExtractResources("panorama/images/econ/loading_screen", "Items");

            modCommon.WriteLine("Extracting Taunts", true);
            DotaResources.ExtractResources("panorama/images/econ/taunts", "Items");

            modCommon.WriteLine("Extracting courier", true);
            DotaResources.ExtractResources("panorama/images/econ/courier", "Items");

            modCommon.WriteLine("Extracting courier", true);
            DotaResources.ExtractResources("panorama/images/econ/music", "Items");

            modCommon.WriteLine("Extracting announcer", true);
            DotaResources.ExtractResources("panorama/images/econ/announcer", "Items");

            modCommon.WriteLine("Extracting terrain", true);
            DotaResources.ExtractResources("panorama/images/econ/terrain", "Items");

            modCommon.WriteLine("Extracting Tools images", true);
            DotaResources.ExtractResources("panorama/images/econ/tools", "Items");

            modCommon.WriteLine("Extracting cursors images", true);
            DotaResources.ExtractResources("panorama/images/econ/cursor_pack", "Items");

            modCommon.WriteLine("Extracting huds images", true);
            DotaResources.ExtractResources("panorama/images/econ/huds", "Items");

            modCommon.WriteLine("Extracting couriers images", true);
            DotaResources.ExtractResources("panorama/images/econ/courier", "Items");

            modCommon.WriteLine("Extracting Items images", true);
            DotaResources.ExtractResources("panorama/images/items", "xxx");

            if (Started)
                return;

            modCommon.WriteLine("Generating program resources", true);
            DotaResources.LoadResources();

            modCommon.WriteLine("Loading Rich Presence", true);
            DotaResources.LoadRichPresence();

            modCommon.WriteLine("Loading items descriptions", true);
            DotaResources.LoadItemsDescriptions();

            Started = true;
        }

        private void ResourcesWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            int x = 11;
            int y = 16;

            if (!manager.Heroes.Any())
            {
                modManager.SetHeroesToList();
            }

            manager.Heroes.Sort((s1, s2) => s1.Name.ToString().CompareTo(s2.Name.ToString()));
            for (int i = 0; i < manager.Heroes.Count; i++)
            {
                try
                {
                    Image HeroPic = ItemsManager.GetImage("heroes/selection/" + manager.Heroes[i].Name);
                    if (HeroPic != null)
                    {
                        tabPage1.Controls.Add(
                        new PictureBox()
                        {
                            Name = manager.Heroes[i].Name,
                            Image = HeroPic,
                            Size = new System.Drawing.Size(43, 59),
                            SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage,
                            Location = new System.Drawing.Point(x, y),
                            Tag = "1"
                        });

                        if (x == 1089)
                        {
                            x = 11;
                            y = y + 65;
                        }
                        else
                            x = x + 49;
                    }
                }
                catch (Exception ex) { modCommon.Save(ex); }


            }
            //Add Click event
            for (int i = 0; i < tabPage1.Controls.Count; i++)
            {
                if (tabPage1.Controls[i] is PictureBox && tabPage1.Controls[i].Name.Contains("npc_dota_hero"))
                {
                    tabPage1.Controls[i].Click += new System.EventHandler(this.Hero_Click);
                    tabPage1.Controls[i].MouseMove += Hero_MouseMove;
                    tabPage1.Controls[i].MouseLeave += Hero_MouseLeave;
                }
            }
            Updating = false;
            SelectTab("tabPage1");
            Ready = true;

            ClientVersion.Visible = true;
            createdBy.Visible = true;
        }


        private void Hero_MouseLeave(object sender, EventArgs e)
        {
            PictureBox pic = (PictureBox)sender;
            pic.BorderStyle = BorderStyle.None;
        }

        private void Hero_MouseMove(object sender, MouseEventArgs e)
        {
            PictureBox pic = (PictureBox)sender;
            pic.BorderStyle = BorderStyle.FixedSingle;
        }

        private void Hero_Click(object sender, EventArgs e)
        {
            PictureBox pic = (PictureBox)sender;

            ProcessHero(pic.Name);
        }

        public void ProcessHero(string Name)
        {
            CleanTabItems();

            modCommon.WriteLine("Cargando", true);

            SetLeftRightHero(Name);

            string videoFile = modCommon.GetVideoFileName(Name);
            if (File.Exists(VideoFolder + @"\" + videoFile))
            {
                media.Open(VideoFolder + @"\" + videoFile, HeroAvatar);
                media.Repeat = true;
                media.Play();
                
            }
            else
                HeroAvatar.Image = modCommon.GetHeroImage(Name);

            //Get Slots
            items.Sort((s1, s2) => s2.item_slot.CompareTo(s1.item_slot));

            int x = 3;
            int y = 5;

            if (Name == "npc_dota_hero_tiny")
                x = 1;

            List<string> Type = new List<string>();
            foreach (var item in items)
            {
                if (item.used_by_heroes == Name)
                {
                    if (!Type.Contains(item.item_slot))
                    {
                        Items itemSlot = ItemsManager.GetSavedItem(item.used_by_heroes, item.item_slot);
                        if (itemSlot == null)
                            itemSlot = new Items() { ItemID = "0000", used_by_heroes = Name, item_slot = item.item_slot};

                        SlotContainer.Controls.Add(
                        new BoxItems()
                        {
                            Name = item.item_slot,
                            Item = itemSlot,
                            Type = item.item_slot.ToUpper(),
                            Hero = Name,
                            Location = new System.Drawing.Point(x, y),
                            Size = new System.Drawing.Size(108, 97),
                            MaximumSize = new System.Drawing.Size(108, 97),
                            MinimumSize = new System.Drawing.Size(108, 97),
                        });
                        x = x + 110;

                        Type.Add(item.item_slot);
                    }
                }
            }

            //Add Taunt
            List<Items> taunts = new List<Items>();
            taunts = Taunts.FindAll(t => t.used_by_heroes == Name);
            if (taunts.Any())
            {
                Items tauntSlot = ItemsManager.GetSavedItem(Name, "taunt");
                if (tauntSlot == null)
                    tauntSlot = new Items() { ItemID = "0000", used_by_heroes = Name, prefab = "taunt", item_slot = "taunt" };

                SlotContainer.Controls.Add(
                new BoxItems()
                {
                    Name = "TAUNT",
                    Item = tauntSlot,
                    Type = "taunt",
                    Hero = Name,
                    Location = new System.Drawing.Point(3, 100),
                    Size = new System.Drawing.Size(108, 97),
                    MaximumSize = new System.Drawing.Size(108, 97),
                    MinimumSize = new System.Drawing.Size(108, 97),
                });
            }




            //Add Bundles

            BundleControl = new MetroPanel();
            this.BundleControl.AutoScroll = true;
            this.BundleControl.HorizontalScrollbar = true;
            this.BundleControl.HorizontalScrollbarBarColor = true;
            this.BundleControl.HorizontalScrollbarHighlightOnWheel = false;
            this.BundleControl.HorizontalScrollbarSize = 10;
            this.BundleControl.Location = new System.Drawing.Point(12, 321);
            this.BundleControl.Name = "BundleControl";
            this.BundleControl.Size = new System.Drawing.Size(1112, 154);
            this.BundleControl.TabIndex = 59;
            this.BundleControl.UseSelectable = false;
            this.BundleControl.VerticalScrollbar = true;
            this.BundleControl.VerticalScrollbarBarColor = true;
            this.BundleControl.VerticalScrollbarHighlightOnWheel = false;
            this.BundleControl.VerticalScrollbarSize = 10;

            int bx = 5;
            int by = 7;

            Bundles.Sort((s1, s2) => s2.ItemID.CompareTo(s1.ItemID));

            foreach (Items bundle in Bundles)
            {
                if (bundle.used_by_heroes == Name)
                {
                    BundleControl.Controls.Add(
                    new BundleBox()
                    {
                        item = bundle,
                        ShowToolTip = true,
                        Location = new System.Drawing.Point(bx, by)
                    });
                    bx = bx + 135;
                }
            }


            if (!tabPage2.Controls.Contains(BundleControl))
                tabPage2.Controls.Add(BundleControl);

            HeroName.Text = ItemsManager.GetHeroName(Name);
            AudioPlayer.PlayHeroName(Name);
            SelectTab("tabPage2");

        }

        private void SetLeftRightHero(string name)
        {
            for (int i = 0; i < manager.Heroes.Count; i++)
            {
                if (manager.Heroes[i].Name == name)
                {
                    if (i == 0)
                    {
                        labelLeft.Text = ItemsManager.GetHeroName(manager.Heroes[manager.Heroes.Count - 1].Name);
                        labelRight.Text = ItemsManager.GetHeroName(manager.Heroes[i + 1].Name);
                    }
                    else if (i == manager.Heroes.Count - 1)
                    {
                        labelLeft.Text = ItemsManager.GetHeroName(manager.Heroes[i - 1].Name);
                        labelRight.Text = ItemsManager.GetHeroName(manager.Heroes[0].Name);
                    }
                    else
                    {
                        labelLeft.Text = ItemsManager.GetHeroName(manager.Heroes[i - 1].Name);
                        labelRight.Text = ItemsManager.GetHeroName(manager.Heroes[i + 1].Name);
                    }
                }
            }
        }

        public void Bundle_Click(BundleBox box)
        {
            switch (box.item.prefab.ToLower())
            {
                case "bundle":
                    List<Items> BundleItems = new List<Items>();

                    foreach (string itemn in box.item.bundles)
                    {
                        foreach (Items itemname in items)
                        {
                            if (itemname.name.ToLower() == itemn.ToLower())
                                BundleItems.Add(itemname);
                        }
                    }
                    
                    for (int i = 0; i < SlotContainer.Controls.Count; i++)
                    {
                        if (SlotContainer.Controls[i] is BoxItems)
                        {
                            try
                            {
                                BoxItems item = (BoxItems)SlotContainer.Controls[i];

                                Items it = BundleItems.Find(x => x.item_slot.ToLower() == item.Name.ToLower());
                                
                                if (it != null)
                                {
                                    item.Item = it;
                                }
                            }
                            catch (Exception ex) { modCommon.Save(ex); }

                        }
                    }
                    break;
                case "music":
                    if (box.isTool)
                    {
                        CurrentMusicPack.item = box.item;
                        manager.MusicPack = box.item;


                        if (string.IsNullOrEmpty(box.item.asset))
                        {
                            if (box.item.prefab == "music")
                            {
                                if (box.item.item_name.Contains("The_International"))
                                {
                                    string name = box.item.item_name;
                                    name = name.Replace("#DOTA_Item_The_International_201", "");
                                    name = name.Replace("_Music_Pack", "");

                                    string path = "sounds/music/valve_ti" + name + "/music/ui_hero_select.vsnd";
                                    AudioPlayer.soundPlayer.Stop();
                                    AudioPlayer.PlaySound(path, true, 34);
                                }
                                
                            }
                        }
                        else
                        {
                            AudioPlayer.soundPlayer.Stop();
                            AudioPlayer.PlaySound(box.item.asset, true, 34);
                        }
                    }
                    else
                        ShowToolsBox(box);
                    break;
                case "announcer":
                    if (box.isTool)
                    {
                        if (box.item.item_slot == "announcer")
                        {
                            CurrentAnnouncer.item = box.item;
                            manager.Announcer = box.item;

                            if (string.IsNullOrEmpty(box.item.asset))
                            {
                                string file = "";
                                ItemsManager.GetSoundAssetName(box.item.ItemID, out string announ);
                                if (!string.IsNullOrEmpty(announ))
                                {
                                    string path = announ.Replace("npc_dota_hero_", "");
                                    path = "sounds/vo/" + path;
                                    file = announ.Replace("npc_dota_hero_announcer_dlc_", "");
                                    file = file + "_ann_welcome_01";
                                    AudioPlayer.soundPlayer.Stop();
                                    AudioPlayer.PlaySound(path + "/" + file + ".vsnd", true, 186);
                                }
                                else
                                {
                                    string hero = box.item.item_description.Replace("#DOTA_Item_Desc_Announcer_", "");
                                    string path = "sounds/vo/announcer_dlc_" + hero;
                                    file = announ.Replace("npc_dota_hero_announcer_dlc_", "");
                                    file = file + "_ann_welcome_01";
                                    AudioPlayer.soundPlayer.Stop();
                                    AudioPlayer.PlaySound(path + "/" + file + ".vsnd", true, 186);

                                }

                            }
                            else
                            {
                                AudioPlayer.soundPlayer.Stop();
                                AudioPlayer.PlaySound(box.item.asset, true, 186);
                            }
                        }
                        else if (box.item.item_slot == "mega_kills")
                        {
                            CurrentMegaKill.item = box.item;
                            manager.MegaKillAnnouncer = box.item;

                            if (string.IsNullOrEmpty(box.item.asset))
                            {
                                
                                string file = "";
                                ItemsManager.GetSoundAssetName(box.item.ItemID, out string announ);
                                string path = announ.Replace("npc_dota_hero_", "");
                                path = "sounds/vo/" + path;
                                file = announ.Replace("npc_dota_hero_announcer_dlc_", "");
                                file = file + "_ann_welcome_01";
                                AudioPlayer.soundPlayer.Stop();
                                AudioPlayer.PlaySound(path + "/" + file + ".vsnd", true, 341);
                            }
                            else
                            {
                                AudioPlayer.soundPlayer.Stop();
                                AudioPlayer.PlaySound(box.item.asset, true, 341);
                            }
                        }
                    }
                    else
                        ShowToolsBox(box);
                    
                    break;
                case "terrain":
                    if (box.isTool)
                    { 
                        CurrentTerrain.item = box.item;
                        manager.Terrain = box.item;
                    }
                    else
                        ShowToolsBox(box);
                    break;
                case "misc":
                    if (box.item.item_slot == "multikill_banner")
                    {
                        if (box.isTool)
                        { 
                            CurrentMultikillBanner.item = box.item;
                            manager.MultikillBanner = box.item;
                        }
                        else
                            ShowToolsBox(box);
                    }
                    else if (box.item.item_slot == "weather")
                    {
                        if (box.isTool)
                        { 
                            CurrentWeatherEffect.item = box.item;
                            manager.WeatherEffect = box.item;
                        }
                        else
                            ShowToolsBox(box);
                    }
                    break;
                case "radiantcreeps":
                    if (box.isTool)
                    { 
                        CurrentRadiantCreeps.item = box.item;
                        manager.RadiantCreeps = box.item;
                    }
                    else
                        ShowToolsBox(box);
                    break;
                case "direcreeps":
                    if (box.isTool)
                    { 
                        CurrentDireCreeps.item = box.item;
                        manager.DireCreeps = box.item;
                    }
                    else
                        ShowToolsBox(box);
                    break;
                case "radianttowers":
                    if (box.isTool)
                    { 
                        CurrentRadiantTowers.item = box.item;
                        manager.RadiantTowers = box.item;
                    }
                    else
                        ShowToolsBox(box);
                    break;
                case "diretowers":
                    if (box.isTool)
                    { 
                        CurrentDireTowers.item = box.item;
                        manager.DireTowers = box.item;
                    }
                    else
                        ShowToolsBox(box);
                    break;
                case "versus_screen":
                    if (box.isTool)
                    { 
                        CurrentVersusScreen.item = box.item;
                        manager.VersusScreen = box.item;
                    }
                    else
                        ShowToolsBox(box);
                    break;
                case "cursor_pack":
                    if (box.isTool)
                    { 
                        CurrentCursorPack.item = box.item;
                        manager.CursorPack = box.item;
                    }
                    else
                        ShowToolsBox(box);
                    break;
                case "emblem":
                    if (box.isTool)
                    { 
                        CurrentEmblem.item = box.item;
                        manager.Emblem = box.item;
                    }
                    else
                        ShowToolsBox(box);
                    break;
                case "emoticon_tool":
                    if (box.isTool)
                    { 
                        CurrentEmoticons.item = box.item;
                        manager.Emoticons = box.item;
                    }
                    else
                        ShowToolsBox(box);
                    break;
                case "hud_skin":
                    if (box.isTool)
                    { 
                        CurrentHUDSkin.item = box.item;
                        manager.HUDSkin = box.item;
                    }
                    else
                        ShowToolsBox(box);
                    break;
                case "courier":
                    if (box.isTool)
                    { 
                        CurrentCourier.item = box.item;
                        manager.Courier = box.item;
                    }
                    else
                        ShowToolsBox(box);
                    break;
                case "ward":
                    if (box.isTool)
                    {
                        CurrentWard.item = box.item;
                        manager.Ward = box.item;
                    }
                    else
                        ShowToolsBox(box);
                    break;
                case "wearable":
                    if (box.isTool)
                    {
                        if (box.item.image_inventory.ToLower().Contains("/pets/"))
                        {
                            CurrentPets.item = box.item;
                            manager.Pets = box.item;
                        }
                    }
                    else
                        ShowToolsBox(box);
                    break;
                case "default_item":
                    if (box.isTool)
                    {
                        /*if (CurrentWard.item.item_type_name.ToLower().Contains("_pet"))
                        {
                            CurrentWard.item = box.item;
                            manager.Ward = box.item;
                        }*/
                    }
                    else
                        ShowToolsBox(box);
                    break;
                case "streak_effect":
                    if (box.isTool)
                    {
                        CurrentStreakEffect.item = box.item;
                        manager.StreakEffect = box.item;
                    }
                    else
                        ShowToolsBox(box);
                    break;
                default:
                    break;
            }
        }
        private void ShowToolsBox(BundleBox box)
        {
            frmLoading loading = new frmLoading(box.item);
            loading.ShowDialog();
        }
        private void CleanTabItems()
        {
            SlotContainer.Controls.Clear();

            if (BundleControl != null)
                tabPage2.Controls.Remove(BundleControl);
        }


        private void FrmMain_MouseMove(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < tabPage1.Controls.Count; i++)
            {
                if (tabPage1.Controls[i] is PictureBox && tabPage1.Controls[i].Name.Contains("npc_dota_hero"))
                {
                    PictureBox pic = (PictureBox)tabPage1.Controls[i];
                    pic.BorderStyle = BorderStyle.None;
                }
            }
        }
        public void MostrarLabel(string v)
        {
            load.Visible = true;
            //Width
            int W_Pantalla = Width / 2; //680
            int AnchoTexto = Convert.ToInt32(modCommon.GetTextSize(v, load.Font).Width) / 2;
            int WidthText = W_Pantalla - AnchoTexto;

            //Heigth
            int HeigthText = Height - 300;

            load.Location = new Point(WidthText, HeigthText);
            load.Text = v;
        }
        public void MostrarKey(string v)
        {
            //Width
            int W_Pantalla = Width / 2; //680
            int AnchoTexto = Convert.ToInt32(modCommon.GetTextSize(v, keyPressed.Font).Width) / 2;
            int WidthText = W_Pantalla - AnchoTexto;

            //Heigth
            int HeigthText = Height - 133;

            keyPressed.Location = new Point(WidthText, HeigthText);
            keyPressed.Text = v;
        }


        private void Menu_Click(object sender, EventArgs e)
        {
            if (Updating)
                return;
            Label label = new Label();
            if (sender is Label)
                label = (Label)sender;
            else if (sender is Panel)
            {
                foreach (var item in ((Panel)sender).Controls)
                {
                    if (item is Label)
                        label = (Label)item;
                }
            }

            switch (label.Text)
            {
                case "HEROES":
                    CleanTabItems();
                    foreach (var control in tabPage1.Controls)
                    {
                        if (control is PictureBox)
                        {
                            ((PictureBox)control).Image = modCommon.GetHeroImage(((PictureBox)control).Name);
                            ((PictureBox)control).Tag = "1";
                        }
                    }
                    SelectTab("tabPage1");
                    break;
                case "MISC":
                    if (tabPage3Loaded)
                        SelectTab("tabPage3");
                    else
                        ProcessMISCTab();
                    break;
                case "WORLD":
                    if (tabPage4Loaded)
                        SelectTab("tabPage4");
                    else
                        ProcessWORDTab();
                    break;
                case "CREATE MOD":
                    manager.CreateMod();
                    break;
            }
        }

        private void ProcessWORDTab()
        {
            //Terrain
            if (manager.Terrain != null) CurrentTerrain.item = manager.Terrain;
            else
            {
                CurrentTerrain.item = Terrain.Find(x => x.name.ToLower().Contains("default"));
                if (CurrentTerrain.item == null) CurrentTerrain.item = new Items() { prefab = "terrain" };
            }

            //Weather Effect
            if (manager.WeatherEffect != null) CurrentWeatherEffect.item = manager.WeatherEffect;
            else
            {
                CurrentWeatherEffect.item = WeatherEffect.Find(x => x.name.ToLower().Contains("default"));
                if (CurrentWeatherEffect.item == null) CurrentWeatherEffect.item = new Items() { prefab = "misc", item_slot = "weather" };
            }

            //Radiant Tower
            if (manager.RadiantTowers != null) CurrentRadiantTowers.item = manager.RadiantTowers;
            else
            {
                CurrentRadiantTowers.item = RadiantTowers.Find(x => x.name.ToLower().Contains("default"));
                if (CurrentRadiantTowers.item == null) CurrentRadiantTowers.item = new Items() { prefab = "radianttowers" };
            }

            //Dire Tower
            if (manager.DireTowers != null) CurrentDireTowers.item = manager.DireTowers;
            else
            {
                CurrentDireTowers.item = DireTowers.Find(x => x.name.ToLower().Contains("default"));
                if (CurrentDireTowers.item == null) CurrentDireTowers.item = new Items() { prefab = "diretowers" };
            }

            //Radiant Creeps
            if (manager.RadiantCreeps != null) CurrentRadiantCreeps.item = manager.RadiantCreeps;
            else
            {
                CurrentRadiantCreeps.item = RadiantCreeps.Find(x => x.name.ToLower().Contains("default"));
                if (CurrentRadiantCreeps.item == null) CurrentRadiantCreeps.item = new Items() { prefab = "radiantcreeps" };
            }

            //Dire Creeps
            if (manager.DireCreeps != null) CurrentDireCreeps.item = manager.DireCreeps;
            else
            {
                CurrentDireCreeps.item = DireCreeps.Find(x => x.name.ToLower().Contains("default"));
                if (CurrentDireCreeps.item == null) CurrentDireCreeps.item = new Items() { prefab = "direcreeps" };
            }

            //VersusScreen
            if (manager.VersusScreen != null) CurrentVersusScreen.item = manager.VersusScreen;
            else
            {
                CurrentVersusScreen.item = VersusScreen.Find(x => x.name.ToLower().Contains("default"));
                if (CurrentVersusScreen.item == null) CurrentVersusScreen.item = new Items() { prefab = "versus_screen" };
            }

            //CurrentCursorPack
            if (manager.CursorPack != null) CurrentCursorPack.item = manager.CursorPack;
            else
            {
                CurrentCursorPack.item = CursorPack.Find(x => x.name.ToLower().Contains("default"));
                if (CurrentCursorPack.item == null) CurrentCursorPack.item = new Items() { prefab = "cursor_pack" };
            }

            //CurrentMultikillBanner
            if (manager.MultikillBanner != null) CurrentMultikillBanner.item = manager.MultikillBanner;
            else
            {
                CurrentMultikillBanner.item = MultikillBanner.Find(x => x.name.ToLower().Contains("default"));
                if (CurrentMultikillBanner.item == null) CurrentMultikillBanner.item = new Items() { prefab = "misc", item_slot = "multikill_banner", item_rarity = "", name = "Default Multikill-Banner" };
            }

            //CurrentEmblem
            if (manager.Emblem != null) CurrentEmblem.item = manager.Emblem;
            else
            {
                CurrentEmblem.item = Emblem.Find(x => x.name.ToLower().Contains("default"));
                if (CurrentEmblem.item == null) CurrentEmblem.item = new Items() { prefab = "emblem" };
            }

            //CurrentEmoticons
            if (manager.Emoticons != null) CurrentEmoticons.item = manager.Emoticons;
            else
            {
                CurrentEmoticons.item = Emoticons.Find(x => x.name.ToLower().Contains("default"));
                if (CurrentEmoticons.item == null) CurrentEmoticons.item = new Items() { prefab = "emoticon_tool" };
            }

            //CurrentHudSkin
            if (manager.HUDSkin != null) CurrentHUDSkin.item = manager.HUDSkin;
            else
            {
                CurrentHUDSkin.item = HUDSkin.Find(x => x.name.ToLower().Contains("default"));
                if (CurrentHUDSkin.item == null) CurrentHUDSkin.item = new Items() { prefab = "hud_skin" };
            }

            //CurrentCourier
            if (manager.Courier != null) CurrentCourier.item = manager.Courier;
            else
            {
                CurrentCourier.item = Courier.Find(x => x.name.ToLower().Contains("default"));
                if (CurrentCourier.item == null) CurrentCourier.item = new Items() { prefab = "courier" };
            }

            //CurrentWard
            if (manager.Ward != null) CurrentWard.item = manager.Ward;
            else
            {
                CurrentWard.item = Ward.Find(x => x.name.ToLower().Contains("default"));
                if (CurrentWard.item == null) CurrentWard.item = new Items() { prefab = "ward" };
            }

            tabPage4Loaded = true;
            SelectTab("tabPage4");

        }
        private void Effect_Click(object sender, EventArgs e)
        {

        }
        private void ProcessMISCTab()
        {
            DetectExternalMods();

            //MusicPack
            if (manager.MusicPack != null) CurrentMusicPack.item = manager.MusicPack;
            else
            {
                CurrentMusicPack.item = MusicPack.Find(x => x.name.ToLower().Contains("default"));
                if (CurrentMusicPack.item == null) CurrentMusicPack.item = new Items() { prefab = "music" };
            }

            //Announcer
            if (manager.Announcer != null) CurrentAnnouncer.item = manager.Announcer;
            else
            {
                CurrentAnnouncer.item = Announcer.Find(x => x.name.ToLower().Contains("default"));
                if (CurrentAnnouncer.item == null) CurrentAnnouncer.item = new Items() { prefab = "announcer", item_slot = "announcer" };
            }

            //Mega-Kill Announcer
            if (manager.MegaKillAnnouncer != null) CurrentMegaKill.item = manager.MegaKillAnnouncer;
            else
            {
                CurrentMegaKill.item = MegaKillAnnouncer.Find(x => x.name.ToLower().Contains("default"));
                if (CurrentMegaKill.item == null) CurrentMegaKill.item = new Items() { prefab = "announcer", item_slot = "mega_kills" };
            }
            //Pets
            if (manager.Pets != null) CurrentPets.item = manager.Pets;
            else
            {
                CurrentPets.item = DefaultItems.Find(x => x.item_name.ToLower().Contains("defaultpet"));
                if (CurrentPets.item == null) CurrentPets.item = new Items() { prefab = "wearable", item_slot = "summon", item_name = "#DOTA_Wearable_All_Heroes_DefaultPet" };
            }

            //streak_effect
            if (manager.StreakEffect != null) CurrentStreakEffect.item = manager.StreakEffect;
            else
            {
                CurrentStreakEffect.item = StreakEffect.Find(x => x.item_name.ToLower().Contains("No Kill"));
                if (CurrentStreakEffect.item == null) CurrentStreakEffect.item = new Items() { name = "No Kill Streak Effect", prefab = "streak_effect", image_inventory = "econ/default_no_item", item_name = "#DOTA_Item_No_Kill_Streak_Effect" };
            }



            //LoadingScreen
            foreach (Items screen in LoadingScreen)
            {
                if (screen.name.ToLower().Contains("default"))
                {
                    CurrentLoadingScreen.item = screen;
                    LoadingScreenContainer.BackColor = modCommon.ColorFromRarity(screen.item_rarity);
                    loadScreen.Image = ItemsManager.GetImage(screen.image_inventory);
                    loadScreen.Tag = screen.name;
                    SelectIndex(screen.name);
                }
                AddItem(screen);
            }
            if (manager.LoadingScreen != null)
            {
                CurrentLoadingScreen.item = manager.LoadingScreen;

                LoadingScreenContainer.BackColor = modCommon.ColorFromRarity(manager.LoadingScreen.item_rarity);
                loadScreen.Image = ItemsManager.GetImage(manager.LoadingScreen.image_inventory);
                loadScreen.Tag = manager.LoadingScreen.name;
                SelectIndex(manager.LoadingScreen.name);
            }

            tabPage3Loaded = true;
            SelectTab("tabPage3");
        }
        private void SelectIndex(string name)
        {
            return;
            for (int i = 0; i < LoadingScreenView.Items.Count; i++)
            {
                //LoadingScreenView.SelectedIndex = i;
                if (LoadingScreenView.Text == name)
                    return;
            }
        }
        private void AddItem(Items screen)
        {
            ListViewItem.ListViewSubItem user;
            ListViewItem.ListViewSubItem pnam;
            ListViewItem.ListViewSubItem gal;
            ListViewItem.ListViewSubItem sis;
            ListViewItem.ListViewSubItem pnt;

            ListViewItem listItem = LoadingScreenView.FindItemWithText(screen.name);

            if (listItem != null)
            {
                return;
            }

            ListViewItem listViewItem = new ListViewItem();

            user = new ListViewItem.ListViewSubItem();
            pnam = new ListViewItem.ListViewSubItem();
            gal = new ListViewItem.ListViewSubItem();
            sis = new ListViewItem.ListViewSubItem();
            pnt = new ListViewItem.ListViewSubItem();

            listViewItem.SubItems.Add(user);
            listViewItem.SubItems.Add(pnam);
            listViewItem.SubItems.Add(gal);
            listViewItem.SubItems.Add(sis);
            listViewItem.SubItems.Add(pnt);

            listViewItem.SubItems[0].Text = "";
            listViewItem.SubItems[0].Tag = screen.image_inventory;
            listViewItem.SubItems[1].Text = screen.name;
            listViewItem.SubItems[2].Text = screen.ItemID;
            listViewItem.SubItems[3].Text = screen.item_rarity;
            listViewItem.SubItems[3].ForeColor = modCommon.ColorFromRarity(screen.item_rarity);


            if (!LoadingScreenView.Items.Contains(listViewItem))
                LoadingScreenView.Items.Add(listViewItem);
        }
        private void LoadingScreenView_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string ImageInventory = LoadingScreenView.SelectedItems[0].SubItems[0].Tag.ToString();
                string rarity = LoadingScreenView.SelectedItems[0].SubItems[3].Text;
                LoadingScreenContainer.BackColor = modCommon.ColorFromRarity(rarity);
                loadScreen.Image = ItemsManager.GetImage(ImageInventory);
                loadScreen.Tag = LoadingScreenView.SelectedItems[0].SubItems[1].Text;
            }
            catch (Exception ex)
            {
                if (ex.Message != "El valor de '0' no es válido para 'index'")
                {
                    modCommon.Save(ex);
                }
            }
        }
        private void LoadingScreenView_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                Items item = ItemsManager.GetItem(LoadingScreenView.SelectedItems[0].SubItems[2].Text);
                CurrentLoadingScreen.item = item;
                manager.LoadingScreen = item;
            }
            catch (Exception ex) { modCommon.Save(ex); }
        }
        private void Event_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                Location = new Point((Location.X - lastLocation.X) + e.X, (Location.Y - lastLocation.Y) + e.Y);
                Update();
                Opacity = 0.93;
            }

            for (int i = 0; i < tabPage1.Controls.Count; i++)
            {
                if (tabPage1.Controls[i] is PictureBox && tabPage1.Controls[i].Name.Contains("npc_dota_hero"))
                {
                    PictureBox pic = (PictureBox)tabPage1.Controls[i];
                    pic.BorderStyle = BorderStyle.None;
                }
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


        private void CloseBtn_MouseMove(object sender, MouseEventArgs e)
        {
            CloseBtn.Image = Resources.Close2;
        }

        private void CloseBtn_MouseLeave(object sender, EventArgs e)
        {
            CloseBtn.Image = Resources.Close_1;
        }

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            Settings.Save();


            manager.Save();
            hook.Remove();

            Process.GetCurrentProcess().Kill();
        }
        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            manager.Save();
            hook.Remove();
        }
        private void CursorWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                for (int i = 0; i < Controls.Count; i++)
                {
                    try
                    {
                        Icon a;
                        a = Properties.Resources.cursor;
                        Controls[i].Cursor = new Cursor(a.Handle);
                        this.Cursor = new Cursor(a.Handle);
                        createdBy.Cursor = new Cursor(a.Handle);
                        HeroAvatar.Cursor = new Cursor(a.Handle);
                    }
                    catch (Exception ex) { modCommon.Save(ex); }
                }

            }
        }

        private void Settings_MouseMove(object sender, MouseEventArgs e)
        {
            OpenSettings.Image = Resources.Settings2;
        }

        private void Settings_MouseLeave(object sender, EventArgs e)
        {
            OpenSettings.Image = Resources.Settings1;
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            if (!Ready)
                return;

            frmSettings settings = new frmSettings();
            settings.Show();
        }

        private void TabPage3_Click(object sender, EventArgs e)
        {

        }

        private void Logo_Click(object sender, EventArgs e)
        {
            frm.tabControl1.SelectTab("tabPage3");

        }



        private void HeroLeft_MouseMove(object sender, MouseEventArgs e)
        {
            picLeft.Image = Resources.Left2;
            labelLeft.ForeColor = Color.FromArgb(114, 129, 134);
        }

        private void HeroLeft_MouseLeave(object sender, EventArgs e)
        {
            picLeft.Image = Resources.Left1;
            labelLeft.ForeColor = Color.FromArgb(55, 64, 69);
        }
        private void HeroRight_MouseMove(object sender, MouseEventArgs e)
        {
            picRight.Image = Resources.Rigth2;
            labelRight.ForeColor = Color.FromArgb(114, 129, 134);
        }

        private void HeroRight_MouseLeave(object sender, EventArgs e)
        {
            picRight.Image = Resources.Rigth1;
            labelRight.ForeColor = Color.FromArgb(55, 64, 69);
        }
        private void HeroLeft_Click(object sender, EventArgs e)
        {
            AudioPlayer.PlaySound("sounds/ui/panorama/panorama_topmenu_select_01.vsnd", false);
            SelectTab("tabPage6");
            string Hero = ItemsManager.GetHeroOriginalName(labelLeft.Text);
            ProcessHero(Hero);
        }
        private void HeroRight_Click(object sender, EventArgs e)
        {
            AudioPlayer.PlaySound("sounds/ui/panorama/panorama_topmenu_select_01.vsnd", false);
            SelectTab("tabPage6");
            string Hero = ItemsManager.GetHeroOriginalName(labelRight.Text);
            ProcessHero(Hero);
        }

        private void LabelRight_TextChanged(object sender, EventArgs e)
        {
            int AnchoOriginal = Convert.ToInt32(modCommon.GetTextSize("IO", load.Font).Width);

            int AnchoTexto = Convert.ToInt32(modCommon.GetTextSize(labelRight.Text, load.Font).Width);

            int OriginalX = 1085;
            int CurrentX = OriginalX - (AnchoTexto - AnchoOriginal);

            labelRight.Location = new Point(CurrentX, labelRight.Location.Y);
        }

        private void OpenDota_Click(object sender, EventArgs e)
        {
            try { Process.GetProcessesByName("dota2")[0].Kill(); } catch { }

            try
            {
                string dota2ExEs = Path.Combine(Paths.GetParent(Settings.Dota2Path), "bin", "win32") + "/dota2.exe";
                Process.Start(dota2ExEs);
            }
            catch (Exception ex) { modCommon.Save(ex); }

        }

        private void CleanControlWorker_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void HeroName_Click(object sender, EventArgs e)
        {
            
        }
        public static void SetBoxInfo(BundleBox bundleBox)
        {
            Control control = bundleBox.Parent; 
            frm.boxInfo.item = bundleBox.item;
            frm.boxInfo.BringToFront();
            int X = control.Location.X + bundleBox.Location.X + (bundleBox.Width + 10);
            if (X > 980)
            {
                X = X - ((bundleBox.Width * 2) + 32);
            }
            frm.boxInfo.Location = new Point(X, control.Location.Y + bundleBox.Location.Y + bundleBox.Height - 63);

            if (frm.boxInfo.SetVisible)
                frm.boxInfo.Visible = true;
        }

        private void CreatedBy_Click(object sender, EventArgs e)
        {
            if (Environment.UserName == "Hackerprod")
            {

                //return;
            }

            frmAbout about = new frmAbout();
            about.Show();

        }

        private void ImportBtn_Click(object sender, EventArgs e)
        {
            var openDialog = new OpenFileDialog
            {
                Title = "Select zip file to import",
                Filter = "Zip file | *.zip",
                Multiselect = true,
            };
            DialogResult userOK = openDialog.ShowDialog();

            if (userOK == DialogResult.OK)
            {
                string[] files = openDialog.FileNames;

                foreach (string file in files)
                {
                    try
                    {
                        Paths.EnsureDirectory(modCommon.TempDirectory);

                        if (Directory.Exists(modCommon.TempDirectory))
                            Paths.CleanDirectory(modCommon.TempDirectory);

                        ZipFile.ExtractToDirectory(file, modCommon.TempDirectory);

                        string[] copyFiles = Directory.GetFiles(modCommon.TempDirectory, "*.*", SearchOption.AllDirectories);
                        for (int i = 0; i < copyFiles.Length; i++)
                        {
                            string sourceDirName = copyFiles[i];
                            string destDirName = copyFiles[i].Replace(modCommon.TempDirectory, modCommon.DataDirectory + "/ExternalItems");

                            Paths.EnsureDirectory(Path.GetDirectoryName(destDirName));

                            try { File.Copy(sourceDirName, destDirName, true); } catch { }
                        }

                        if (Directory.Exists(modCommon.TempDirectory))
                            Paths.CleanDirectory(modCommon.TempDirectory);

                        Directory.Delete(modCommon.TempDirectory, true);

                    }
                    catch (Exception ex) { modCommon.Save(ex); }
                }
            }
            DetectExternalMods();
        }

        private void DetectExternalMods()
        {
            ExternalModView.Items.Clear();
            ExternalModView.UseSelectable = true;
            string detectedItems = "";
            string[] Files = Directory.GetFiles(modCommon.DataDirectory + "/ExternalItems", "*.vpcf_c", SearchOption.AllDirectories);
            foreach (var file in Files)
            {
                string item = Path.GetFileNameWithoutExtension(file);
                detectedItems += "Detected " + item.Replace("_", " ") + " particle" + Environment.NewLine;
                AddExternalMod("Detected " + item.Replace("_", " ") + " particle");
            }
            
        }
        private void AddExternalMod(string externalmod)
        {
            var mod = new ListViewItem.ListViewSubItem();
            ListViewItem listViewItem = new ListViewItem();
            listViewItem.SubItems.Add(mod);
            listViewItem.SubItems[0].Text = externalmod;
            ExternalModView.Items.Add(listViewItem);
        }

        private void FlatButton1_Click(object sender, EventArgs e)
        {
            Paths.CleanDirectory(modCommon.DataDirectory + "/ExternalItems");
            DetectExternalMods();
        }

        private void ExportThis_Click(object sender, EventArgs e)
        {
            LoadingScreenManager.Extract(CurrentLoadingScreen.item);
        }

        private void ExportAll_Click(object sender, EventArgs e)
        {
            LoadingScreenManager.Extract();
        }

        private void SetDesktop_Click(object sender, EventArgs e)
        {
            try
            {
                Items item = CurrentLoadingScreen.item;
                string filename = item.asset.Replace("panorama/images/", "");
                filename = ItemsManager.NameFromAsset(filename);
                string filePath = modCommon.DataDirectory + "/Loading Screen/" + filename + ".png";
                if (File.Exists(filePath))
                {
                    Wallpaper.Set(filePath, Wallpaper.Style.Stretched);
                }
                else
                {
                    LoadingScreenManager.Extract(item, true);
                }
            }
            catch (Exception ex) { modCommon.Save(ex); }
        }

        private void Menu_MouseMove(object sender, MouseEventArgs e)
        {
            if (Updating)
                return;

            if (sender is Label)
            {
                ((Label)sender).ForeColor = Color.FromArgb(224, 224, 224);
            }
            else if (sender is Panel)
            {
                foreach (Control item in ((Panel)sender).Controls)
                {
                    if (item is Label)
                    {
                        ((Label)item).ForeColor = Color.FromArgb(224, 224, 224);
                    }
                }
            }
            if (sender is Label)
            {
                /*
                if (((Label)sender) == createdBy)
                {
                    boxInfo.item_name.Text = "Skynet Dota2Mod";
                    boxInfo.item_description.Text = "Created by Hackerprod" +
                        Environment.NewLine + "hackerprodlive@gmail.com" +
                        Environment.NewLine + "www.facebook.com/hackerprod"
                        ;

                    this.boxInfo.Location = new System.Drawing.Point(995, 385);
                    this.boxInfo.Visible = true;
                }
                */
            }
        }
        private void Menu_MouseLeave(object sender, EventArgs e)
        {
            if (Updating)
                return;

            if (sender is Label)
            {
                switch (CurrentTab)
                {
                    case "tabPage1":
                        if (((Label)sender) == labelHeroes) return; else ((Label)sender).ForeColor = Color.Gray;
                        break;
                    case "tabPage2":
                        if (((Label)sender) == labelHeroes) return; else ((Label)sender).ForeColor = Color.Gray;
                        break;
                    case "tabPage3":
                        if (((Label)sender) == labelMisc) return; else ((Label)sender).ForeColor = Color.Gray;
                        break;
                    case "tabPage4":
                        if (((Label)sender) == labelWord) return; else ((Label)sender).ForeColor = Color.Gray;
                        break;
                    default:
                        ((Label)sender).ForeColor = Color.Gray;
                        break;
                }
            }
            else if (sender is Panel)
            {
                foreach (Control item in ((Panel)sender).Controls)
                {
                    if (item is Label)
                    {
                        switch (CurrentTab)
                        {
                            case "tabPage1":
                                if (((Label)item) == labelHeroes) return; else ((Label)item).ForeColor = Color.Gray;
                                break;
                            case "tabPage2":
                                if (((Label)item) == labelHeroes) return; else ((Label)item).ForeColor = Color.Gray;
                                break;
                            case "tabPage3":
                                if (((Label)item) == labelMisc) return; else ((Label)item).ForeColor = Color.Gray;
                                break;
                            case "tabPage4":
                                if (((Label)item) == labelWord) return; else ((Label)item).ForeColor = Color.Gray;
                                break;
                            default:
                                ((Label)item).ForeColor = Color.Gray;
                                break;
                        }
                    }
                }
            }
            if (sender is Label)
            {
                if (((Label)sender) == createdBy)
                {
                    this.boxInfo.Visible = false;
                }
            }
        }


        public static void SelectTab(string tabPage)
        {
            if (tabPage != "tabPage6")
                CurrentTab = tabPage;

            frm.tabControl1.SelectTab(tabPage);
            frm.SetMenuLabelColor(tabPage);

            if (tabPage != "tabPage6" && tabPage != "tabPage2")
                AudioPlayer.PlaySound("sounds/ui/panorama/panorama_topmenu_select_03.vsnd", false);

            switch (tabPage)
            {
                case "tabPage1":
                    frm.banner.Image = Resources.M1;
                    break;
                case "tabPage2":
                    frm.banner.Image = Resources.M1;
                    frm.KeyBind.Focus();
                    break;
                case "tabPage4":
                    frm.banner.Image = Resources.M2;
                    break;
                case "tabPage3":
                    frm.banner.Image = Resources.M3;
                    break;

                case "tabPage6":
                    frm.logo.Visible = true;
                    frm.logo.BringToFront();
                    break;

                default:
                    frm.banner.Image = Resources.Banner;
                    break;
            }
        }
        private void SetMenuLabelColor(string tabPage)
        {
            Label label = new Label();
            switch (tabPage)
            {
                case "tabPage1":
                    label = labelHeroes;
                    break;
                case "tabPage2":
                    label = labelHeroes;
                    break;
                case "tabPage3":
                    label = labelMisc;
                    break;
                case "tabPage4":
                    label = labelWord;
                    break;
                default:
                    break;
            }
            //modCommon.Show(label.Text);
            foreach (Control item in Menus)
            {
                if (item is Label)
                {
                    if (item.Text == label.Text)
                    {
                        item.ForeColor = Color.FromArgb(224, 224, 224);
                    }
                    else
                    {
                        item.ForeColor = Color.Gray;
                    }
                }
            }

        }

        private void StopSounds_Click(object sender, EventArgs e)
        {
            AudioPlayer.soundPlayer.Stop();
        }

        private void KeyPress_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void KeyPress_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete || e.KeyData == Keys.Back || e.KeyData == Keys.Return || e.KeyData == Keys.Escape)
            {
                foreach (var control in tabPage1.Controls)
                {
                    if (control is PictureBox)
                    {
                        ((PictureBox)control).Image = modCommon.GetHeroImage(((PictureBox)control).Name);
                        ((PictureBox)control).Tag = "1";
                    }
                }

            }
            else
            {
                string key = e.KeyData.ToString().ToLower();

                if (key.Length > 1)
                    return;

                MostrarKey(key.ToUpper());
                keyTimer.Enabled = true;
                foreach (var control in tabPage1.Controls)
                {
                    if (control is PictureBox)
                    {
                        PictureBox hero = (PictureBox)control;
                        string NamePrepared = hero.Name.Replace("npc_dota_hero_", "");
                        NamePrepared = NamePrepared.Replace("_", " ");

                        if (NamePrepared.ToLower().StartsWith(key))
                        {
                            ((PictureBox)control).Image = modCommon.GetHeroImage(((PictureBox)control).Name);
                            ((PictureBox)control).Tag = "1";
                        }
                        else
                        {
                            if (hero.Tag.ToString() == "1")
                            {
                                hero.Image = modCommon.ChangeOpacity(hero.Image, 0.2f);
                                hero.Tag = "0";
                            }
                        }
                    }
                }
            }
            keyPress.Text = "";
        }

        private void KeyTimer_Tick(object sender, EventArgs e)
        {
            MostrarKey("");
            keyTimer.Enabled = false;
        }

        private void LoadScreen_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                string filename = loadScreen.Tag.ToString();
                string filePath = modCommon.DataDirectory + "/Loading Screen/" + filename + ".png";
                Items item = new Items() { name = filename };

                if (File.Exists(filePath))
                {
                    Process.Start(filePath);
                }
                else
                {
                    LoadingScreenManager.Extract(item, open: true);
                }
            }
            catch (Exception ex) { modCommon.Save(ex); }
        }

        private void BoxInfo_Click(object sender, EventArgs e)
        {
            
        }

        private void KeyBind_KeyDown(object sender, KeyEventArgs e)
        {
            //ProcessKey(e.KeyData);
        }

        public void ProcessKey(Keys keyData)
        {
            if (keyData == Keys.Left)
            {
                if (frm.tabControl1.SelectedTab == tabPage2)
                {
                    AudioPlayer.PlaySound("sounds/ui/panorama/panorama_topmenu_select_01.vsnd", false);
                    SelectTab("tabPage6");
                    string Hero = ItemsManager.GetHeroOriginalName(labelLeft.Text);
                    ProcessHero(Hero);
                }
            }
            else if (keyData == Keys.Right)
            {
                if (frm.tabControl1.SelectedTab == tabPage2)
                {
                    AudioPlayer.PlaySound("sounds/ui/panorama/panorama_topmenu_select_01.vsnd", false);
                    SelectTab("tabPage6");
                    string Hero = ItemsManager.GetHeroOriginalName(labelRight.Text);
                    ProcessHero(Hero);
                }
            }
            else if (keyData == Keys.F5)
            {
                //Compilar el programa
                if (Ready)
                {
                    manager.CreateMod();
                }
            }
            else if (keyData == Keys.F9)
            {
                //Abrir el Dota
                if (Ready)
                {
                    try { Process.GetProcessesByName("dota2")[0].Kill(); } catch { }

                    try
                    {
                        modCommon.WriteLine("Opening Dota2", true);
                        string dota2ExE = Path.Combine(Paths.GetParent(Settings.Dota2Path), "bin", "win32") + "/dota2.exe";
                        Process.Start(dota2ExE);
                    }
                    catch { }

                }
            }
        }

        private void OpenImageMenu_Click(object sender, EventArgs e)
        {
            try
            {
                string filename = loadScreen.Tag.ToString();
                string filePath = modCommon.DataDirectory + "/Loading Screen/" + filename + ".png";
                Items item = new Items() { name = filename };

                if (File.Exists(filePath))
                {
                    Process.Start(filePath);
                }
                else
                {
                    LoadingScreenManager.Extract(item, open: true);
                }
            }
            catch (Exception ex) { modCommon.Save(ex); }
        }

        private void SaveImageMenu_Click(object sender, EventArgs e)
        {
            try
            {
                string filename = loadScreen.Tag.ToString();
                string filePath = modCommon.DataDirectory + "/Loading Screen/" + filename + ".png";
                Items item = new Items() { name = filename };

                LoadingScreenManager.Extract(item);
            }
            catch (Exception ex) { modCommon.Save(ex); }
        }

        private void LoadScreen_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int W_loadScreen = loadScreen.Width / 2;
                int AnchoMenu = OpenImageMenu.Width /2;
                int LocationX = W_loadScreen - AnchoMenu;

                //Heigth
                int LocationY = (loadScreen.Height /2) - OpenImageMenu.Height;

                ImageMenu.Show(loadScreen, LocationX, LocationY);
            }
        }


    }
}
