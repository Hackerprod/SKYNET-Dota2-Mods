using SKYNET;
using SkynetDota2Mods.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SkynetDota2Mods
{
    public partial class frmSettings : Form
    {
        private bool mouseDown;     //Mover ventana
        private Point lastLocation; //Mover ventana

        public frmSettings()
        {
            InitializeComponent();
            ActiveSounds.CheckedChanged += ActiveSounds_CheckedChanged; 
        }

        private void ActiveSounds_CheckedChanged(object sender)
        {
            if (!ActiveSounds.Checked)
                AudioPlayer.StopSounds();
        }

        private void CloseBox_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CloseBox_MouseMove(object sender, MouseEventArgs e)
        {
            Settings.GenerateOnStart = GenerateOnStart.Checked;
            Settings.Dota2Path = Dota2Path.Text;
            Settings.OpenDota = OpenDota.Checked;
            Settings.ActiveSounds = ActiveSounds.Checked;

            CloseBox.Image = Resources.SClose2;
        }

        private void CloseBox_MouseLeave(object sender, EventArgs e)
        {
            CloseBox.Image = Resources.SClose1;
        }

        private void FrmSettings_Load(object sender, EventArgs e)
        {
            AudioPlayer.PlaySound("sounds/ui/panorama/panorama_topmenu_select_03.vsnd", false);
            GenerateOnStart.Checked = Settings.GenerateOnStart;
            Dota2Path.Text = Settings.Dota2Path;
            OpenDota.Checked = Settings.OpenDota;
            ActiveSounds.Checked = Settings.ActiveSounds;

            ItemsToReset.Items.Add("");
            ItemsToReset.Items.Add("Announcer");
            ItemsToReset.Items.Add("Mega-Kill Pack");
            ItemsToReset.Items.Add("Multi-kill Banner");
            ItemsToReset.Items.Add("Courier");
            ItemsToReset.Items.Add("Cursor Pack");
            ItemsToReset.Items.Add("Dire Creeps");
            ItemsToReset.Items.Add("Dire Towers");
            ItemsToReset.Items.Add("Radiant Creeps");
            ItemsToReset.Items.Add("Radiant Towers");
            ItemsToReset.Items.Add("Music Pack");
            ItemsToReset.Items.Add("Pets");
            ItemsToReset.Items.Add("Versus Screen");
            ItemsToReset.Items.Add("Ward");
            ItemsToReset.Items.Add("Weather Effect");
            ItemsToReset.Items.Add("Loading Screen");
            ItemsToReset.Items.Add("Emblem");
            ItemsToReset.Items.Add("Emoticons");
            ItemsToReset.Items.Add("Hud");
            ItemsToReset.Items.Add("Terrain");
            ItemsToReset.Items.Add("Streak Effect");

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
                    }
                    catch { }
                }
            }
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            frmMain.manager.Save();
        }

        private void CheckCache_Click(object sender, EventArgs e)
        {
            frmMain.frm.ResourcesWorker.RunWorkerAsync();
        }

        private void ChangeBtn_Click(object sender, EventArgs e)
        {
            try
            {
                var openDialog = new OpenFileDialog
                {
                    Title = "Select pak01_dir.vpk in \"" + @"dota 2 beta\game\dota" + "\"",
                    Filter = "Valve File | pak01_dir.vpk",
                    Multiselect = false,
                };
                DialogResult userOK = openDialog.ShowDialog();

                if (userOK == DialogResult.OK)
                {
    //                modCommon.Settings = Path.GetDirectoryName(openDialog.FileName);
                    modCommon.VPKLocation = openDialog.FileName;
                    Dota2Path.Text = openDialog.FileName;
                }
            }
            catch (Exception)
            {

                modCommon.Show("Error searching location, please run program with admin privilegies");
            }
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

        private void FrmSettings_Deactivate(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DeleteMod_Click(object sender, EventArgs e)
        {
             string vpkPath = Paths.GetParent(Settings.Dota2Path).ToString() + "/SkynetMod";

            //Preparar el VPK
            if (Directory.Exists(vpkPath))
            {
                try { Directory.Delete(vpkPath, true); } catch { }
            }

            modHelpers.ClearGameInfo();
        }

        private void FrmSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            AudioPlayer.PlaySound("sounds/ui/panorama/panorama_topmenu_select_01.vsnd", false);
        }


        private void Reset_Click(object sender, EventArgs e)
        {
            switch (ItemsToReset.Text)
            {
                case "Announcer":
                    frmMain.manager.Announcer = null;
                    frmMain.frm.CurrentAnnouncer.Reset();
                    break;
                case "Mega-Kill Pack":
                    frmMain.manager.MegaKillAnnouncer = null;
                    frmMain.frm.CurrentMegaKill.Reset();
                    break;
                case "Multi-kill Banner":
                    frmMain.manager.MultikillBanner = null;
                    frmMain.frm.CurrentMultikillBanner.Reset();
                    break;
                case "Courier":
                    frmMain.manager.Courier = null;
                    frmMain.frm.CurrentCourier.Reset();
                    break;
                case "Cursor Pack":
                    frmMain.manager.CursorPack = null;
                    frmMain.frm.CurrentCursorPack.Reset();
                    break;
                case "Dire Creeps":
                    frmMain.manager.DireCreeps = null;
                    frmMain.frm.CurrentDireCreeps.Reset();
                    break;
                case "Dire Towers":
                    frmMain.manager.DireTowers = null;
                    frmMain.frm.CurrentDireTowers.Reset();
                    break;
                case "Radiant Creeps":
                    frmMain.manager.RadiantCreeps = null;
                    frmMain.frm.CurrentRadiantCreeps.Reset();
                    break;
                case "Radiant Towers":
                    frmMain.manager.RadiantTowers = null;
                    frmMain.frm.CurrentRadiantTowers.Reset();
                    break;
                case "Music Pack":
                    frmMain.manager.MusicPack = null;
                    frmMain.frm.CurrentMusicPack.Reset();
                    break;
                case "Pets":
                    frmMain.manager.Pets = null;
                    frmMain.frm.CurrentPets.Reset();
                    break;
                case "Versus Screen":
                    frmMain.manager.VersusScreen = null;
                    frmMain.frm.CurrentVersusScreen.Reset();
                    break;
                case "Ward":
                    frmMain.manager.Ward = null;
                    frmMain.frm.CurrentWard.Reset();
                    break;
                case "Weather Effect":
                    frmMain.manager.WeatherEffect = null;
                    frmMain.frm.CurrentWeatherEffect.Reset();
                    break;
                case "Loading Screen":
                    frmMain.manager.LoadingScreen = null;
                    frmMain.frm.CurrentLoadingScreen.Reset();
                    break;
                case "Emblem":
                    frmMain.manager.Emblem = null;
                    frmMain.frm.CurrentEmblem.Reset();
                    break;
                case "Emoticons":
                    frmMain.manager.Emoticons = null;
                    frmMain.frm.CurrentEmoticons.Reset();
                    break;
                case "Hud":
                    frmMain.manager.HUDSkin = null;
                    frmMain.frm.CurrentHUDSkin.Reset();
                    break;
                case "Terrain":
                    frmMain.manager.Terrain = null;
                    frmMain.frm.CurrentTerrain.Reset();
                    break;
                case "Streak Effect":
                    frmMain.manager.StreakEffect = null;
                    frmMain.frm.CurrentStreakEffect.Reset();
                    break;
            }
        }

        private void SETTINGS_Click(object sender, EventArgs e)
        {

        }

        private void Dota2Path_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (Directory.Exists(Dota2Path.Text) && File.Exists(Dota2Path.Text + "/pak01_dir.vpk"))
                {
                    Settings.Dota2Path = Dota2Path.Text;
                    modCommon.VPKLocation = Dota2Path.Text + "/pak01_dir.vpk";
                }
            }
            catch (Exception)
            {
            }

        }
    }
}
