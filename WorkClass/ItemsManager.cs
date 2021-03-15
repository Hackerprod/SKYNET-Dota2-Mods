using SkynetDota2Mods.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;

namespace SkynetDota2Mods
{
    public class ItemsManager
    {
        public static Items GetItem(string itemID)
        {
            Items result = null;
            result = frmMain.frm.items.Find(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.DefaultItems.Find(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.Bundles.Find(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.Towers.Find(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.Taunts.Find(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.MusicPack.Find(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.Announcer.Find(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.Terrain.Find(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.WeatherEffect.Find(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.RadiantCreeps.Find(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.DireCreeps.Find(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.RadiantTowers.Find(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.DireTowers.Find(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.VersusScreen.Find(x => x.ItemID == itemID);

            if (result == null)
                result = frmMain.frm.CursorPack.Find(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.MultikillBanner.Find(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.Emblem.Find(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.Emoticons.Find(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.HUDSkin.Find(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.Courier.Find(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.Ward.Find(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.LoadingScreen.Find(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.MegaKillAnnouncer.Find(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.Pets.Find(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.StreakEffect.Find(x => x.ItemID == itemID);


            return result;
        }

        public static string GetDescription(string name)
        {
            foreach (var item in frmMain.frm.itemDescription)
            {
                if (item.item_description.ToLower() == name.ToLower())
                {
                    return item.description;
                }
            }
            return name;
        }

        public static List<Items> GetItems(string itemID)
        {
            List<Items> result = new List<Items>();
            result = frmMain.frm.items.FindAll(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.DefaultItems.FindAll(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.Bundles.FindAll(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.Towers.FindAll(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.Taunts.FindAll(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.MusicPack.FindAll(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.Announcer.FindAll(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.Terrain.FindAll(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.WeatherEffect.FindAll(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.RadiantCreeps.FindAll(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.DireCreeps.FindAll(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.RadiantTowers.FindAll(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.DireTowers.FindAll(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.VersusScreen.FindAll(x => x.ItemID == itemID);

            if (result == null)
                result = frmMain.frm.CursorPack.FindAll(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.MultikillBanner.FindAll(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.Emblem.FindAll(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.Emoticons.FindAll(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.HUDSkin.FindAll(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.Courier.FindAll(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.Ward.FindAll(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.LoadingScreen.FindAll(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.MegaKillAnnouncer.FindAll(x => x.ItemID == itemID);
            if (result == null)
                result = frmMain.frm.Pets.FindAll(x => x.ItemID == itemID);

            return result;
        }

        internal static string NameFromAsset(string v)
        {

            foreach (Items item in frmMain.frm.LoadingScreen)
            {
                if (item.asset == v.Replace(".vtex", ""))
                {
                    return item.name;
                }
                if (item.asset == v.Replace("_tga.vtex_c", ""))
                {
                    return item.name;
                }
                if (item.asset == v.Replace(".vtex_c", ".vtex"))
                {
                    return item.name;
                }
            }

            string PreparedName = Paths.GetParent(v);
            PreparedName = PreparedName.Replace("loadingscreens", "");
            PreparedName = PreparedName.Replace("/", "");
            PreparedName = PreparedName.Replace("_", " ");
            PreparedName = PreparedName.Replace("loadingscreens", " ");
            PreparedName = PreparedName.Replace("loading", " ");
            PreparedName = PreparedName.Replace("screens", " ");
            return PreparedName;
        }

        public static List<Items> GetItemsFromPrefab(Items item)
        {

            switch (item.prefab)
            {
                case "wearable":
                    if (item.image_inventory.ToLower().Contains("/pets/"))
                        return frmMain.frm.Pets;

                    return frmMain.frm.items;
                case "default_item":
                    if (item.item_name.ToLower().Contains("pet"))
                    {
                        return frmMain.frm.Pets;
                    }
                    else
                        return frmMain.frm.DefaultItems;
                case "bundle":
                    return frmMain.frm.Bundles;
                case "taunt":
                    return frmMain.frm.Taunts;
                case "music":
                    return frmMain.frm.MusicPack;
                case "announcer":
                    if (item.item_slot == "announcer")
                        return frmMain.frm.Announcer;
                    else
                        return frmMain.frm.MegaKillAnnouncer;
                case "terrain":
                    return frmMain.frm.Terrain;
                case "misc":
                    if (item.item_slot == "weather")
                        return frmMain.frm.WeatherEffect;   //WeatherEffect
                    else
                        return frmMain.frm.MultikillBanner; //MultikillBanner
                case "radiantcreeps":
                    return frmMain.frm.RadiantCreeps;
                case "direcreeps":
                    return frmMain.frm.DireCreeps;
                case "radianttowers":
                    return frmMain.frm.RadiantTowers;
                case "diretowers":
                    return frmMain.frm.DireTowers;
                case "versus_screen":
                    return frmMain.frm.VersusScreen;




                case "cursor_pack":
                    return frmMain.frm.CursorPack;
                case "emblem":
                    return frmMain.frm.Emblem;
                case "emoticon_tool":
                    return frmMain.frm.Emoticons;
                case "hud_skin":
                    return frmMain.frm.HUDSkin;
                case "courier":
                    return frmMain.frm.Courier;
                case "ward":
                    return frmMain.frm.Ward;
                case "multikill_banner":
                    return frmMain.frm.MultikillBanner;
                case "loading_screen":
                    return frmMain.frm.LoadingScreen;
                case "streak_effect":
                    return frmMain.frm.StreakEffect;
                default:
                    return new List<Items>();
            }
        }

        internal static Items GetItemFromName(string itemname)
        {
            Items result = null;
            result = frmMain.frm.items.Find(x => x.name.ToLower() == itemname.ToLower());
            if (result == null)
                result = frmMain.frm.DefaultItems.Find(x => x.name.ToLower() == itemname.ToLower());
            if (result == null)
                result = frmMain.frm.Bundles.Find(x => x.name.ToLower() == itemname.ToLower());
            if (result == null)
                result = frmMain.frm.Towers.Find(x => x.name.ToLower() == itemname.ToLower());
            if (result == null)
                result = frmMain.frm.Taunts.Find(x => x.name.ToLower() == itemname.ToLower());
            if (result == null)
                result = frmMain.frm.MusicPack.Find(x => x.name.ToLower() == itemname.ToLower());
            if (result == null)
                result = frmMain.frm.Announcer.Find(x => x.name.ToLower() == itemname.ToLower());
            if (result == null)
                result = frmMain.frm.Terrain.Find(x => x.name.ToLower() == itemname.ToLower());
            if (result == null)
                result = frmMain.frm.WeatherEffect.Find(x => x.name.ToLower() == itemname.ToLower());
            if (result == null)
                result = frmMain.frm.RadiantCreeps.Find(x => x.name.ToLower() == itemname.ToLower());
            if (result == null)
                result = frmMain.frm.DireCreeps.Find(x => x.name.ToLower() == itemname.ToLower());
            if (result == null)
                result = frmMain.frm.RadiantTowers.Find(x => x.name.ToLower() == itemname.ToLower());
            if (result == null)
                result = frmMain.frm.DireTowers.Find(x => x.name.ToLower() == itemname.ToLower());
            if (result == null)
                result = frmMain.frm.VersusScreen.Find(x => x.name.ToLower() == itemname.ToLower());
            if (result == null)
                result = frmMain.frm.CursorPack.Find(x => x.name.ToLower() == itemname.ToLower());
            if (result == null)
                result = frmMain.frm.MultikillBanner.Find(x => x.name.ToLower() == itemname.ToLower());
            if (result == null)
                result = frmMain.frm.Emblem.Find(x => x.name.ToLower() == itemname.ToLower());
            if (result == null)
                result = frmMain.frm.Emoticons.Find(x => x.name.ToLower() == itemname.ToLower());
            if (result == null)
                result = frmMain.frm.HUDSkin.Find(x => x.name.ToLower() == itemname.ToLower());
            if (result == null)
                result = frmMain.frm.Courier.Find(x => x.name.ToLower() == itemname.ToLower());
            if (result == null)
                result = frmMain.frm.Ward.Find(x => x.name.ToLower() == itemname.ToLower());
            if (result == null)
                result = frmMain.frm.LoadingScreen.Find(x => x.name.ToLower() == itemname.ToLower());
            if (result == null)
                result = frmMain.frm.StreakEffect.Find(x => x.name.ToLower() == itemname.ToLower());
            return result;
        }
        internal static Items GetBundleFromName(string itemname)
        {
            Items result = null;
            foreach (Items item in frmMain.frm.Bundles)
            {
                if (item.name.ToLower() == itemname)
                    return item;
            }
            return result;
        }


        internal static Image GetDefaultImageItem(string slot, string hero)
        {
            if (slot == "taunt")
                return Resources.default_taunt;

            try
            {
                List<Items> defaultItem = new List<Items>();

                foreach (Items item in frmMain.frm.DefaultItems)
                {
                    if (item.used_by_heroes == hero)
                    {
                        defaultItem.Add(item);
                    }
                }
                Items item2 = new Items();
                foreach (Items item in defaultItem)
                {
                    item2 = item;
                    if (item.item_slot.ToLower() == slot.ToLower())
                    {
                        return ItemsManager.GetImage(item2.image_inventory);
                    }
                }
                //Get real directory
                string[] adta = item2.image_inventory.Split('/');
                string direction = "";
                for (int i = 0; i < adta.Length; i++)
                {
                    if (i != adta.Length - 1)
                        direction += adta[i] + "/";
                }

                string[] files = Directory.GetFiles("data/panorama/images/" + direction);

                bool continues = true;
                int cont = 0;
                while (continues)
                {
                    foreach (var file in files)
                    {
                        for (int i = 0; i < defaultItem.Count; i++)
                        {
                            Items item = defaultItem[i];
                            if (file.Contains(item.image_inventory))
                            {
                                defaultItem.Remove(item);
                            }
                            if (defaultItem.Count == 1)
                            {
                                return ItemsManager.GetImage(defaultItem[0].image_inventory);
                            }
                        }
                    }
                    cont++;
                    if (cont > 10)
                        continues = false;
                }
            }
            catch (Exception)
            {
                return Resources.default_item;
            }
            return Resources.default_item;
        }


        internal static Items GetSavedItem(string used_by_heroes, string item_slot)
        {
            return frmMain.manager.Heroes.Find(x => x.Name == used_by_heroes).Bundles.Find(x => x.item_slot == item_slot);
        }

        public static string GetHeroName(string Name)
        {
            string result = "";
            foreach (var item in frmMain.frm.rich_presence)
            {
                if (item.item_description.ToLower() == "#" + Name.ToLower())
                {
                    result = item.description;
                }
            }

            if (result == "")
                result = Name.Replace("npc_dota_hero_", "").Replace("_", " ");

            return result.ToUpper();
        }

        internal static string GetHeroOriginalName(string Name)
        {
            string result = "";
            foreach (var item in frmMain.frm.rich_presence)
            {
               
                if (item.description.ToLower() == Name.ToLower())
                {
                    result = item.item_description;
                }
            }

            if (result == "")
                result = "npc_dota_hero_" + Name.Replace(" ", "_").ToLower(); ;

            return result.Replace("#", "").ToLower();
        }
        internal static Image GetImage(string path)
        {
            //modCommon.Show("data/panorama/images/" + path + ".png");

            if (File.Exists("data/panorama/images/" + path + ".png"))
            {
                return Image.FromFile("data/panorama/images/" + path + ".png");
            }
            else
            {
                try
                {
                    return DotaResources.ExtractAndGenerate(path);
                }
                catch (Exception)
                {
                    return Resources.default_item;
                }
                

                
            }
        }

        internal static void GetSoundAssetName(string itemID, out string patch)
        {
            patch = "";

            KValue Update = new KValue();
            Update.Children = DotaResources.kValue["Items"][itemID].Children;

            if (Update.ContainsKey("visuals"))
            {
                List<KValue>.Enumerator visuals = Update["visuals"].Children.GetEnumerator();
                while (visuals.MoveNext())
                {
                    KValue currentvisuals = visuals.Current;

                    if (currentvisuals.Name.StartsWith("asset_modifier"))
                    {
                        List<KValue>.Enumerator assets = Update["visuals"][currentvisuals.Name].Children.GetEnumerator();
                        while (assets.MoveNext())
                        {
                            if (assets.Current.Name == "asset")
                            {
                                patch = assets.Current.Value.ToString();
                            }
                        }
                    }
                }
            }
        }
    }
}