using SkiaSharp.Views.Desktop;
using SKYNET;
using SkynetDota2Mods;
using SkynetDota2Mods.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValveResourceFormat;
using ValveResourceFormat.ResourceTypes;

public class DotaResources
{
    public static void ExtractResources_original(string ruta, string ToFolder)
    {
        if (!Settings.GenerateOnStart)
            return;

        string text3 = Path.Combine(modCommon.DataDirectory);
        Paths.EnsureDirectory(text3);
        if (modCommon.package != null)
        {
            try
            {
                try
                {
                    List<PackageEntry> list = (from e in modCommon.package.Entries["vtex_c"]
                                               where e.DirectoryName.Contains(ruta)
                                               select e).ToList();
                    if (list.Any())
                    {
                        foreach (PackageEntry item in list)
                        {
                            string text4 = Path.Combine(text3, item.DirectoryName);
                            string text5 = Path.Combine(text4, item.FileName.Replace("_png", string.Empty) + ".png");
                            //modCommon.Show(text5);
                            if (!File.Exists(text5))
                            {
                                Paths.EnsureDirectory(text4);
                                byte[] bytes = default(byte[]);
                                modCommon.package.ReadEntry(item, out bytes, false);
                                Resource resource = new Resource();
                                try
                                {
                                    using (MemoryStream memoryStream = RecyclableStreams.Create(bytes))
                                    {
                                        resource.Read((Stream)memoryStream);
                                        try
                                        {

                                            ValveResourceFormat.ResourceTypes.Texture texture = (ValveResourceFormat.ResourceTypes.Texture)resource.Blocks[BlockType.DATA];
                                            modCommon.Show(texture.GenerateBitmap().ToBitmap().Width);








                                            using (Bitmap bitmap = ((Texture)resource.Blocks[BlockType.DATA]).GenerateBitmap().ToBitmap())
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

                                                        bitmap2.Save(text5, ImageFormat.Png);

                                                    }
                                                }
                                                else
                                                {
                                                    bitmap.Save(text5, ImageFormat.Png);
                                                }
                                            }
                                        }
                                        catch (Exception ex) { modCommon.Save(ex); }
                                    }
                                }
                                finally
                                {
                                    ((IDisposable)resource)?.Dispose();
                                }
                            }
                        }
                    }
                }
                finally
                {
                    ((IDisposable)modCommon.package)?.Dispose();
                }
            }
            catch (Exception ex) { modCommon.Save("Error extracting econ images.", ex); }

            modCommon.WriteLine("Econ images was extracted successfully.", true);
        }
        else
        {
            modCommon.WriteLine("Dota 2 was not found on content folders...", true);
        }
    }
    public static void ExtractResources(string ruta, string ToFolder)
    {
        if (!modCommon.NeedUpdate)
        if (!Settings.GenerateOnStart)
            return;

        string text3 = Path.Combine(modCommon.DataDirectory);
        Paths.EnsureDirectory(text3);
        if (modCommon.package != null)
        {
            List<PackageEntry> list2 = (from e in modCommon.package.Entries["vtex_c"]
                                       where (e.DirectoryName != null)
                                       select e).ToList();
            List<PackageEntry> list = (from e in list2
                                       where (e.DirectoryName.Contains(ruta))
                                       select e).ToList();
            if (list.Any())
            {
                foreach (PackageEntry item in list)
                {
                    string text4 = Path.Combine(text3, item.DirectoryName);
                    string text5 = Path.Combine(text4, item.FileName.Replace("_png", string.Empty) + ".png");

                    if (!File.Exists(text5))
                    {

                        try
                        {
                            modCommon.package.ReadEntry(item, out byte[] output);

                            Resource resource = new Resource();
                            resource.Read(new MemoryStream(output));
                            ValveResourceFormat.ResourceTypes.Texture texture = (ValveResourceFormat.ResourceTypes.Texture)resource.Blocks[BlockType.DATA];
                            Paths.EnsureDirectory(text4);
                            texture.GenerateBitmap().ToBitmap().Save(text5);
                        }
                        catch (Exception ex) { modCommon.Save(ex); }
                    }
                }
            }
            modCommon.WriteLine("Econ images was extracted successfully.", true);
        }
        else
        {
            modCommon.WriteLine("Dota 2 was not found on content folders...", true);
        }
    }
    public static void ExtractFile(string file, string folder)
    {
        try
        {
            Paths.EnsureDirectory(folder);

            string extention = Path.GetExtension(file);
            extention = extention.Replace(".", "");
            string filePath = Path.GetFullPath(file);

            if (modCommon.package != null)
            {
                if (extention.Contains("vmap"))
                    return;

                PackageEntry package = modCommon.package.Entries[extention + "_c"].Find(x => file.Contains(x.DirectoryName) && file.Contains(x.FileName));

                if (package != null)
                {
                    string path = Path.Combine(folder, package.DirectoryName);
                    string filepath = Path.Combine(path, package.FileName + "." + extention + "_c");

                    if (!File.Exists(filepath))
                    {
                        try
                        {
                            Paths.EnsureDirectory(path);
                            modCommon.package.ReadEntry(package, out byte[] output);
                            File.WriteAllBytes(filepath, output);
                        }
                        catch (Exception ex) { modCommon.Save(ex); }
                    }
                }
            }
        }
        catch (Exception ex) { modCommon.Save(ex); }
    }

    public static KValue kValue;

    internal static void LoadResources()
    {
        //Item list
        List<KValue>.Enumerator enumerator;
        List<KValue>.Enumerator usedbyheroes;
        List<string> ListHeros = new List<string>();

        kValue = KValue.LoadAsText(Path.Combine("data", "db", "items_game.txt"));

        if (kValue != null)
        {
            //prefabs para los valores default (ejemplo cuando no trae item_slot ver el por defecto)
            if (kValue.ContainsKey("prefabs"))
            {
                
                enumerator = kValue["prefabs"].Children.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        KValue current2 = enumerator.Current;
                        //Get Colors
                        prefabs item = new prefabs()
                        {
                            Name = current2.Name,
                            item_quality = current2["item_quality"].AsString(),
                            item_rarity = current2["item_rarity"].AsString(),
                            item_slot = current2["item_slot"].AsString()
                        };
                        frmMain.frm.prefabs.Add(item);
                    }
                }
                finally
                {
                    ((IDisposable)enumerator).Dispose();
                }
            }

            //Get original colors
            if (kValue.ContainsKey("colors"))
            {
                enumerator = kValue["colors"].Children.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        KValue current2 = enumerator.Current;
                        //Get Colors
                        Colors item = new Colors()
                        {
                            Name = current2.Name,
                            color_name = current2["color_name"].AsString(),
                            hex_color = current2["hex_color"].AsString()
                        };
                        frmMain.frm.Colors.Add(item);
                    }
                }
                finally
                {
                    ((IDisposable)enumerator).Dispose();
                }
            }

            if (kValue.ContainsKey("items"))
            {
                enumerator = kValue["items"].Children.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        KValue current2 = enumerator.Current;
                        string UsedBy = "";
                        if (current2.ContainsKey("used_by_heroes"))
                        {
                            usedbyheroes = current2["used_by_heroes"].Children.GetEnumerator();
                            try
                            {
                                while (usedbyheroes.MoveNext())
                                {
                                    KValue current3 = usedbyheroes.Current;
                                    UsedBy = current3.Name;
                                }
                            }
                            catch (Exception ex) { modCommon.Save(ex); }
                        }

                        //Get Styles of Item
                        string Id = "";
                        string Icon_path = "";
                        string Asset = "";
                        string soundAsset = "";
                        List<Style> ItemStyles = new List<Style>();
                        ItemStyles.Clear();
                        if (current2.ContainsKey("visuals"))
                        {
                            List<KValue>.Enumerator visuals = current2["visuals"].Children.GetEnumerator();
                            try
                            {
                                while (visuals.MoveNext())
                                {
                                    KValue currentvisuals = visuals.Current;

                                    if (currentvisuals.Name == "styles")
                                    {
                                        List<KValue>.Enumerator style = current2["visuals"]["styles"].Children.GetEnumerator();
                                        try
                                        {
                                            while (style.MoveNext())
                                            {
                                                Id = style.Current.Name;
                                                Icon_path = current2["visuals"]["alternate_icons"][style.Current.Name]["icon_path"].AsString();

                                                ItemStyles.Add(new Style() { ID = Id, icon_path = Icon_path });
                                            }
                                        }
                                        catch (Exception ex) { modCommon.Save(ex); }
                                    }

                                    Icon_path = "";
                                    soundAsset = "";



                                    //Get asset for loading screen
                                    if (currentvisuals.Name.StartsWith("asset_modifier"))
                                    {
                                        List<KValue>.Enumerator assets = current2["visuals"][currentvisuals.Name].Children.GetEnumerator();
                                        try
                                        {
                                            while (assets.MoveNext())
                                            {
                                                if (assets.Current.Name == "asset")
                                                {
                                                    Asset = assets.Current.Value.ToString();

                                                    if (assets.Current.Value.ToString().Contains(".vsnd"))
                                                    {
                                                        soundAsset = assets.Current.Value.ToString();
                                                        goto Label_01;
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception ex) { modCommon.Save(ex); }
                                    }

                                }

                            }
                            catch (Exception ex) { modCommon.Save(ex); }
                        }
                    Label_01:;

                        //Get wearable item
                        if (current2["prefab"].AsString() == "wearable")
                        {
                            //Save item in list
                            Items item = new Items()
                            {//weapon
                                ItemID = current2.Name,
                                name = current2["name"].AsString(),
                                styles = ItemStyles,
                                prefab = current2["prefab"].AsString(),
                                image_inventory = current2["image_inventory"].AsString(),
                                item_description = current2["item_description"].AsString(),
                                item_name = current2["item_name"].AsString(),
                                item_rarity = modCommon.GetRarityFromPrefabs(current2["item_rarity"].AsString(), "wearable"),
                                item_slot = modCommon.GetSlotFromPrefabs(current2["item_slot"].AsString(), "wearable"),
                                item_type_name = current2["item_type_name"].AsString(),
                                used_by_heroes = UsedBy
                            };

                            if (current2["item_type_name"].AsString().ToLower().Contains("_pet"))
                            {
                                frmMain.frm.Pets.Add(item);
                                //Create null Pets
                                Items Pets = frmMain.frm.Pets.Find(x => x.ItemID == "0000");
                                if (Pets == null) frmMain.frm.Pets.Add(new Items() { item_slot = "summon", prefab = "wearable", image_inventory = "/pets/", name = "All Heroes' Default Pet", ItemID = "0000", item_rarity = "" });

                            }
                            else
                            {
                                frmMain.frm.items.Add(item);
                            }
                        }

                        //Get default_item
                        if (current2["name"].AsString().StartsWith("Default "))
                        {
                            Items item = new Items()
                            {
                                ItemID = current2.Name,
                                name = current2["name"].AsString(),
                                prefab = current2["prefab"].AsString(),
                                image_inventory = current2["image_inventory"].AsString(),
                                item_description = current2["item_description"].AsString(),
                                item_name = current2["item_name"].AsString(),
                                item_rarity = modCommon.GetRarityFromPrefabs(current2["item_rarity"].AsString(), "default_item"),
                                item_slot = modCommon.GetSlotFromPrefabs(current2["item_slot"].AsString(), "default_item"),
                                item_type_name = current2["item_type_name"].AsString(),
                            };

                            frmMain.frm.DefaultItems.Add(item);
                        }

                        if (current2["prefab"].AsString() == "default_item")
                        {
                            Items item = new Items()
                            {
                                ItemID = current2.Name,
                                name = current2["name"].AsString(),
                                prefab = current2["prefab"].AsString(),
                                image_inventory = current2["image_inventory"].AsString(),
                                item_description = current2["item_description"].AsString(),
                                item_name = current2["item_name"].AsString(),
                                item_rarity = modCommon.GetRarityFromPrefabs(current2["item_rarity"].AsString(), "default_item"),
                                item_slot = modCommon.GetSlotFromPrefabs(current2["item_slot"].AsString(), "default_item"),
                                item_type_name = current2["item_type_name"].AsString(),
                                //styles = ItemStyles,
                                used_by_heroes = UsedBy
                            };
                            frmMain.frm.DefaultItems.Add(item);
                        }

                        //Obtener Array de los bundles
                        List<string> bundles = new List<string>();
                        if (current2.ContainsKey("bundle"))
                        {
                            List<KValue>.Enumerator usedbundle = current2["bundle"].Children.GetEnumerator();
                            try
                            {
                                while (usedbundle.MoveNext())
                                {
                                    KValue current3 = usedbundle.Current;
                                    bundles.Add(current3.Name);
                                }
                            }
                            catch (Exception ex) { modCommon.Save(ex); }
                        }


                        //Get Bundles
                        if (current2["prefab"].AsString() == "bundle")
                        {
                            Items item = new Items()
                            {
                                ItemID = current2.Name,
                                bundles = bundles,
                                name = current2["name"].AsString(),
                                prefab = current2["prefab"].AsString(),
                                image_inventory = current2["image_inventory"].AsString(),
                                item_description = current2["item_description"].AsString(),
                                item_name = current2["item_name"].AsString(),
                                item_rarity = modCommon.GetRarityFromPrefabs(current2["item_rarity"].AsString(), "default_item"),
                                item_type_name = current2["item_type_name"].AsString(),
                                used_by_heroes = UsedBy
                            };
                            frmMain.frm.Bundles.Add(item);
                        }
                        //Get Taunts
                        if (current2["prefab"].AsString() == "taunt")
                        {
                            Items item = new Items()
                            {
                                ItemID = current2.Name,
                                styles = ItemStyles,
                                prefab = current2["prefab"].AsString(),
                                name = current2["name"].AsString(),
                                item_slot = "taunt",
                                image_inventory = current2["image_inventory"].AsString(),
                                item_description = current2["item_description"].AsString(),
                                item_name = current2["item_name"].AsString(),
                                item_rarity = modCommon.GetRarityFromPrefabs(current2["item_rarity"].AsString(), "default_item"),
                                used_by_heroes = UsedBy
                            };
                            frmMain.frm.Taunts.Add(item);
                        }
                        //Get music
                        if (current2["prefab"].AsString() == "music")
                        {
                            Items item = new Items()
                            {
                                ItemID = current2.Name,
                                styles = ItemStyles,
                                prefab = current2["prefab"].AsString(),
                                name = current2["name"].AsString(),
                                image_inventory = current2["image_inventory"].AsString(),
                                item_description = current2["item_description"].AsString(),
                                item_name = current2["item_name"].AsString(),
                                item_rarity = modCommon.GetRarityFromPrefabs(current2["item_rarity"].AsString(), "default_item"),
                                asset = soundAsset,
                                used_by_heroes = UsedBy
                            };
                            frmMain.frm.MusicPack.Add(item);
                        }
                        //Get Announcer
                        if (current2["prefab"].AsString() == "announcer")
                        {
                            if (current2["item_slot"].AsString() == "announcer")
                            {
                                Items item = new Items()
                                {
                                    ItemID = current2.Name,
                                    styles = ItemStyles,
                                    prefab = current2["prefab"].AsString(),
                                    name = current2["name"].AsString(),
                                    image_inventory = current2["image_inventory"].AsString(),
                                    item_description = current2["item_description"].AsString(),
                                    item_name = current2["item_name"].AsString(),
                                    item_slot = current2["item_slot"].AsString(),
                                    item_rarity = modCommon.GetRarityFromPrefabs(current2["item_rarity"].AsString(), "default_item"),
                                    asset = soundAsset,
                                    used_by_heroes = UsedBy
                                };
                                frmMain.frm.Announcer.Add(item);
                            }
                            if (current2["item_slot"].AsString() == "mega_kills")
                            {
                                Items item = new Items()
                                {
                                    ItemID = current2.Name,
                                    styles = ItemStyles,
                                    prefab = current2["prefab"].AsString(),
                                    name = current2["name"].AsString(),
                                    image_inventory = current2["image_inventory"].AsString(),
                                    item_description = current2["item_description"].AsString(),
                                    item_name = current2["item_name"].AsString(),
                                    item_slot = current2["item_slot"].AsString(),
                                    item_rarity = modCommon.GetRarityFromPrefabs(current2["item_rarity"].AsString(), "default_item"),
                                    asset = soundAsset,
                                    used_by_heroes = UsedBy
                                };
                                frmMain.frm.MegaKillAnnouncer.Add(item);
                            }
                        }

                        //Get terrain
                        if (current2["prefab"].AsString() == "terrain")
                        {
                            Items item = new Items()
                            {
                                ItemID = current2.Name,
                                styles = ItemStyles,
                                prefab = current2["prefab"].AsString(),
                                name = current2["name"].AsString(),
                                image_inventory = current2["image_inventory"].AsString(),
                                item_description = current2["item_description"].AsString(),
                                item_name = current2["item_name"].AsString(),
                                item_rarity = modCommon.GetRarityFromPrefabs(current2["item_rarity"].AsString(), "default_item"),
                                used_by_heroes = UsedBy
                            };
                            frmMain.frm.Terrain.Add(item);
                        }
                        if (current2["prefab"].AsString() == "misc")
                        {
                            //Get weather effects
                            if (current2["item_slot"].AsString() == "weather")
                            {
                                Items item = new Items()
                                {
                                    ItemID = current2.Name,
                                    styles = ItemStyles,
                                    prefab = "misc",
                                    item_slot = current2["item_slot"].AsString(),
                                    name = current2["name"].AsString(),
                                    image_inventory = current2["image_inventory"].AsString(),
                                    item_description = current2["item_description"].AsString(),
                                    item_name = current2["item_name"].AsString(),
                                    item_rarity = modCommon.GetRarityFromPrefabs(current2["item_rarity"].AsString(), "default_item"),
                                    used_by_heroes = UsedBy
                                };
                                frmMain.frm.WeatherEffect.Add(item);
                            }
                            //Get MultikillBanner
                            if (current2["item_slot"].AsString() == "multikill_banner")
                            {
                                Items item = new Items()
                                {
                                    ItemID = current2.Name,
                                    styles = ItemStyles,
                                    prefab = "misc",
                                    item_slot = current2["item_slot"].AsString(),
                                    name = current2["name"].AsString(),
                                    image_inventory = current2["image_inventory"].AsString(),
                                    item_description = current2["item_description"].AsString(),
                                    item_name = current2["item_name"].AsString(),
                                    item_rarity = modCommon.GetRarityFromPrefabs(current2["item_rarity"].AsString(), "default_item"),
                                    used_by_heroes = UsedBy
                                };
                                frmMain.frm.MultikillBanner.Add(item);

                                //Create null MultikillBanner
                                Items MultikillBanner = frmMain.frm.MultikillBanner.Find(x => x.ItemID == "0000");
                                if (MultikillBanner == null) frmMain.frm.MultikillBanner.Add(new Items() { prefab = "misc", image_inventory = "econ/testitem_slot_empty", name = "Default Multikill-Banner", ItemID = "0000", item_rarity = "", item_slot = "multikill_banner" });
                            }
                        }
                        //Get default weather effects
                        if (current2["prefab"].AsString() == "weather")
                        {
                            Items item = new Items()
                            {
                                ItemID = current2.Name,
                                styles = ItemStyles,
                                prefab = "misc",
                                item_slot = "weather",
                                name = current2["name"].AsString(),
                                image_inventory = current2["image_inventory"].AsString(),
                                item_description = current2["item_description"].AsString(),
                                item_name = current2["item_name"].AsString(),
                                item_rarity = modCommon.GetRarityFromPrefabs(current2["item_rarity"].AsString(), "default_item"),
                                used_by_heroes = UsedBy
                            };
                            frmMain.frm.WeatherEffect.Add(item);

                        }
                        //Get radiantcreeps
                        if (current2["prefab"].AsString() == "radiantcreeps")
                        {
                            Items item = new Items()
                            {
                                ItemID = current2.Name,
                                styles = ItemStyles,
                                prefab = current2["prefab"].AsString(),
                                name = current2["name"].AsString(),
                                image_inventory = current2["image_inventory"].AsString(),
                                item_description = current2["item_description"].AsString(),
                                item_name = current2["item_name"].AsString(),
                                item_rarity = modCommon.GetRarityFromPrefabs(current2["item_rarity"].AsString(), "default_item"),
                                used_by_heroes = UsedBy
                            };
                            frmMain.frm.RadiantCreeps.Add(item);
                        }
                        //Get direcreeps
                        if (current2["prefab"].AsString() == "direcreeps")
                        {
                            Items item = new Items()
                            {
                                ItemID = current2.Name,
                                styles = ItemStyles,
                                prefab = current2["prefab"].AsString(),
                                name = current2["name"].AsString(),
                                image_inventory = current2["image_inventory"].AsString(),
                                item_description = current2["item_description"].AsString(),
                                item_name = current2["item_name"].AsString(),
                                item_rarity = modCommon.GetRarityFromPrefabs(current2["item_rarity"].AsString(), "default_item"),
                                used_by_heroes = UsedBy
                            };
                            frmMain.frm.DireCreeps.Add(item);
                        }
                        //Get RadiantTowers
                        if (current2["prefab"].AsString() == "radianttowers")
                        {
                            Items item = new Items()
                            {
                                ItemID = current2.Name,
                                styles = ItemStyles,
                                prefab = current2["prefab"].AsString(),
                                name = current2["name"].AsString(),
                                image_inventory = current2["image_inventory"].AsString(),
                                item_description = current2["item_description"].AsString(),
                                item_name = current2["item_name"].AsString(),
                                item_rarity = modCommon.GetRarityFromPrefabs(current2["item_rarity"].AsString(), "default_item"),
                                used_by_heroes = UsedBy
                            };
                            frmMain.frm.RadiantTowers.Add(item);
                        }
                        //Get DireTowers
                        if (current2["prefab"].AsString() == "diretowers")
                        {
                            Items item = new Items()
                            {
                                ItemID = current2.Name,
                                styles = ItemStyles,
                                prefab = current2["prefab"].AsString(),
                                name = current2["name"].AsString(),
                                image_inventory = current2["image_inventory"].AsString(),
                                item_description = current2["item_description"].AsString(),
                                item_name = current2["item_name"].AsString(),
                                item_rarity = modCommon.GetRarityFromPrefabs(current2["item_rarity"].AsString(), "default_item"),
                                used_by_heroes = UsedBy
                            };
                            frmMain.frm.DireTowers.Add(item);
                        }

                        //Get versus_screen
                        if (current2["prefab"].AsString() == "versus_screen")
                        {
                            Items item = new Items()
                            {
                                ItemID = current2.Name,
                                styles = ItemStyles,
                                prefab = current2["prefab"].AsString(),
                                name = current2["name"].AsString(),
                                image_inventory = current2["image_inventory"].AsString(),
                                item_description = current2["item_description"].AsString(),
                                item_name = current2["item_name"].AsString(),
                                item_rarity = modCommon.GetRarityFromPrefabs(current2["item_rarity"].AsString(), "default_item"),
                                used_by_heroes = UsedBy
                            };
                            frmMain.frm.VersusScreen.Add(item);
                        }

                        //Get cursor_pack
                        if (current2["prefab"].AsString() == "cursor_pack")
                        {
                            Items item = new Items()
                            {
                                ItemID = current2.Name,
                                styles = ItemStyles,
                                prefab = current2["prefab"].AsString(),
                                name = current2["name"].AsString(),
                                image_inventory = current2["image_inventory"].AsString(),
                                item_description = current2["item_description"].AsString(),
                                item_name = current2["item_name"].AsString(),
                                item_rarity = modCommon.GetRarityFromPrefabs(current2["item_rarity"].AsString(), "default_item"),
                                used_by_heroes = UsedBy
                            };
                            frmMain.frm.CursorPack.Add(item);
                        }

                        //Get cursor_pack
                        if (current2["prefab"].AsString() == "emblem")
                        {
                            Items item = new Items()
                            {
                                ItemID = current2.Name,
                                styles = ItemStyles,
                                prefab = current2["prefab"].AsString(),
                                name = current2["name"].AsString(),
                                image_inventory = current2["image_inventory"].AsString(),
                                item_description = current2["item_description"].AsString(),
                                item_name = current2["item_name"].AsString(),
                                item_rarity = modCommon.GetRarityFromPrefabs(current2["item_rarity"].AsString(), "default_item"),
                                used_by_heroes = UsedBy
                            };
                            frmMain.frm.Emblem.Add(item);

                            //Create null emblem
                            Items emblem = frmMain.frm.Emblem.Find(x => x.ItemID == "0000");
                            if (emblem == null) frmMain.frm.Emblem.Add(new Items() { prefab = "emblem", image_inventory = "econ/testitem_slot_empty", name = "Default Emblem", ItemID = "0000", item_rarity = "" });
                        }

                        //emoticon_tool
                        if (current2["prefab"].AsString() == "emoticon_tool")
                        {
                            Items item = new Items()
                            {
                                ItemID = current2.Name,
                                styles = ItemStyles,
                                prefab = current2["prefab"].AsString(),
                                name = current2["name"].AsString(),
                                image_inventory = current2["image_inventory"].AsString(),
                                item_description = current2["item_description"].AsString(),
                                item_name = current2["item_name"].AsString(),
                                item_rarity = modCommon.GetRarityFromPrefabs(current2["item_rarity"].AsString(), "default_item"),
                                used_by_heroes = UsedBy
                            };
                            frmMain.frm.Emoticons.Add(item);
                        }

                        //hud_skin
                        if (current2["prefab"].AsString() == "hud_skin")
                        {
                            Items item = new Items()
                            {
                                ItemID = current2.Name,
                                styles = ItemStyles,
                                prefab = current2["prefab"].AsString(),
                                name = current2["name"].AsString(),
                                image_inventory = current2["image_inventory"].AsString(),
                                item_description = current2["item_description"].AsString(),
                                item_name = current2["item_name"].AsString(),
                                item_rarity = modCommon.GetRarityFromPrefabs(current2["item_rarity"].AsString(), "default_item"),
                                used_by_heroes = UsedBy
                            };
                            frmMain.frm.HUDSkin.Add(item);
                        }

                        //courier
                        if (current2["prefab"].AsString() == "courier")
                        {
                            Items item = new Items()
                            {
                                ItemID = current2.Name,
                                styles = ItemStyles,
                                prefab = current2["prefab"].AsString(),
                                name = current2["name"].AsString(),
                                image_inventory = current2["image_inventory"].AsString(),
                                item_description = current2["item_description"].AsString(),
                                item_name = current2["item_name"].AsString(),
                                item_rarity = modCommon.GetRarityFromPrefabs(current2["item_rarity"].AsString(), "default_item"),
                                used_by_heroes = UsedBy
                            };
                            frmMain.frm.Courier.Add(item);
                        }

                        //ward
                        if (current2["prefab"].AsString() == "ward")
                        {
                            Items item = new Items()
                            {
                                ItemID = current2.Name,
                                styles = ItemStyles,
                                prefab = current2["prefab"].AsString(),
                                name = current2["name"].AsString(),
                                image_inventory = current2["image_inventory"].AsString(),
                                item_description = current2["item_description"].AsString(),
                                item_name = current2["item_name"].AsString(),
                                item_rarity = modCommon.GetRarityFromPrefabs(current2["item_rarity"].AsString(), "default_item"),
                                used_by_heroes = UsedBy
                            };
                            frmMain.frm.Ward.Add(item);
                        }

                        //loading_screen
                        if (current2["prefab"].AsString() == "loading_screen")
                        {
                            Items item = new Items()
                            {
                                ItemID = current2.Name,
                                styles = ItemStyles,
                                prefab = current2["prefab"].AsString(),
                                name = current2["name"].AsString(),
                                asset = Asset,
                                image_inventory = current2["image_inventory"].AsString(),
                                item_description = current2["item_description"].AsString(),
                                item_name = current2["item_name"].AsString(),
                                item_rarity = modCommon.GetRarityFromPrefabs(current2["item_rarity"].AsString(), "default_item"),
                                used_by_heroes = UsedBy
                            };
                            frmMain.frm.LoadingScreen.Add(item);
                        }

                        //StreakEffect
                        if (current2["prefab"].AsString() == "streak_effect")
                        {
                            Items item = new Items()
                            {
                                ItemID = current2.Name,
                                styles = ItemStyles,
                                prefab = current2["prefab"].AsString(),
                                name = current2["name"].AsString(),
                                asset = Asset,
                                image_inventory = current2["image_inventory"].AsString(),
                                item_description = current2["item_description"].AsString(),
                                item_name = current2["item_name"].AsString(),
                                item_rarity = modCommon.GetRarityFromPrefabs(current2["item_rarity"].AsString(), "default_item"),
                                used_by_heroes = UsedBy
                            };
                            frmMain.frm.StreakEffect.Add(item);
                        }

                        //test
                        /*
                        if (current2["prefab"].AsString() == "misc")
                        {
                            Items item = new Items()
                            {
                                ItemID = current2.Name,
                                prefab = current2["prefab"].AsString(),
                                name = current2["name"].AsString(),
                                image_inventory = current2["image_inventory"].AsString(),
                                item_description = current2["item_description"].AsString(),
                                item_name = current2["item_name"].AsString(),
                                item_rarity = modCommon.GetRarityFromPrefabs(current2["item_rarity"].AsString(), "default_item"),
                                used_by_heroes = UsedBy
                            };
                            frmMain.frm.Emoticons.Add(item);
                        }
                        */
                    }
                }
                finally
                {
                    ((IDisposable)enumerator).Dispose();
                }
            }

            //kValue.SaveToFile("lol.txt", false);


        }

    }
    internal static void LoadItemsDescriptions()
    {
        KValue value;
        if (modCommon.CurrentLanguage == modCommon.Language.Spanish)
            value = KValue.LoadAsText(Path.Combine("data", "db", "lang_spanish.txt"));
        else
            value = KValue.LoadAsText(Path.Combine("data", "db", "lang_english.txt"));

        if (value != null)
        {
            //prefabs para los valores default (ejemplo cuando no trae item_slot ver el por defecto)
            if (value.ContainsKey("Tokens"))
            {
                List<KValue>.Enumerator enumerator = value["Tokens"].Children.GetEnumerator();
                //itemDescription
                try
                {
                    while (enumerator.MoveNext())
                    {
                        KValue current2 = enumerator.Current;

                        ItemDescription item = new ItemDescription();
                        item.item_description = "#" + current2.Name;
                        item.description = current2.Value.ToString();
                        frmMain.frm.itemDescription.Add(item);
                    }
                }
                finally
                {
                    ((IDisposable)enumerator).Dispose();
                }
            }
        }

    }
    internal static void LoadRichPresence()
    {
        KValue kValue = KValue.LoadAsText(Path.Combine(modCommon.DataDirectory, "rich_presence", "570", "spanish.vdf"));
        if (kValue != null)
        {
            //prefabs para los valores default (ejemplo cuando no trae item_slot ver el por defecto)
            if (kValue.ContainsKey("Tokens"))
            {
                List<KValue>.Enumerator enumerator = kValue["Tokens"].Children.GetEnumerator();
                //itemDescription
                try
                {
                    while (enumerator.MoveNext())
                    {
                        KValue current2 = enumerator.Current;

                        ItemDescription item = new ItemDescription();
                        item.item_description = current2.Name;
                        item.description = current2.Value.ToString();
                        frmMain.frm.rich_presence.Add(item);
                    }
                }
                finally
                {
                    ((IDisposable)enumerator).Dispose();
                }
            }
        }

    }
    
    public static Bitmap ExtractAndGenerate(string ruta)
    {
        string filePath = "panorama/images/" + ruta + "_png.vtex_c";

        string text3 = Path.Combine(modCommon.DataDirectory);
        Paths.EnsureDirectory(text3);
        if (modCommon.package != null)
        {
            PackageEntry item = modCommon.package.FindEntry(filePath);

            //modCommon.Show(filePath);
            if (item != null)
            {

                string text4 = Path.Combine(text3, item.DirectoryName);
                string text5 = Path.Combine(text4, item.FileName.Replace("_png", string.Empty) + ".png");

                if (!File.Exists(text5))
                {
                    try
                    {
                        modCommon.package.ReadEntry(item, out byte[] output);

                        Resource resource = new Resource();
                        resource.Read(new MemoryStream(output));
                        ValveResourceFormat.ResourceTypes.Texture texture = (ValveResourceFormat.ResourceTypes.Texture)resource.Blocks[BlockType.DATA];
                        Paths.EnsureDirectory(text4);
                        texture.GenerateBitmap().ToBitmap().Save(text5);
                        return texture.GenerateBitmap().ToBitmap();
                    }
                    catch (Exception ex) { modCommon.Save(ex); }
                }

            }

        }

        return Resources.default_item;
    }
}


