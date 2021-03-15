using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkynetDota2Mods
{
    [Serializable]
    public class Items
    {
        public string ItemID { get; set; }
        public List<string> bundles { get; set; }
        public List<Style> styles { get; set; } = new List<Style>();
        public string name { get; set; }
        public string prefab { get; set; }
        public string image_inventory { get; set; }
        public string item_description { get; set; }
        public string item_name { get; set; }
        public string item_rarity { get; set; }
        public string item_slot { get; set; }
        public string item_type_name { get; set; }
        public Style style { get; set; }
        public string used_by_heroes { get; set; }
        public string asset { get; set; }
       

    }
    public class ListItems
    {
        private static List<Items> Items = new List<Items>();
        public static List<Items> GetItems(string Hero)
        {
            bool IsTheHero = false;
            List<KValue>.Enumerator enumerator;
            List<KValue>.Enumerator usedbyheroes;

            KValue kValue2 = KValue.LoadAsText(Path.Combine("data", "db", "items_game.txt"));

            string toSave = "";

            if (kValue2 != null)
            {
                if (kValue2.ContainsKey("items"))
                {
                    enumerator = kValue2["items"].Children.GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            KValue current2 = enumerator.Current;
                            //uint value = Convert.ToUInt32(current2.Name);
                            string UsedBy = "";
                            if (current2.ContainsKey("used_by_heroes"))
                            {
                                usedbyheroes = current2["used_by_heroes"].Children.GetEnumerator();
                                try
                                {
                                    while (usedbyheroes.MoveNext())
                                    {
                                        KValue current3 = usedbyheroes.Current;
                                        if (current3.Name == Hero)
                                            IsTheHero = true;
                                        else
                                            IsTheHero = true;
                                    }
                                }
                                catch { }
                            }
                            if (IsTheHero)
                            if (!string.IsNullOrEmpty(current2["image_inventory"].AsString()))
                            {
                                Items item = new Items()
                                {
                                    ItemID = current2.Name,
                                    name = current2["name"].AsString(),
                                    prefab = current2["prefab"].AsString(),
                                    image_inventory = current2["image_inventory"].AsString(),
                                    item_description = current2["item_description"].AsString(),
                                    item_name = current2["item_name"].AsString(),
                                    item_rarity = current2["item_rarity"].AsString(),
                                    item_slot = current2["item_slot"].AsString(),
                                    item_type_name = current2["item_type_name"].AsString(),
                                    used_by_heroes = UsedBy
                                };
                                Items.Add(item);
                            }
                        }
                    }
                    finally
                    {
                        ((IDisposable)enumerator).Dispose();
                    }
                }
            }
            return Items;
        }
    }

}
