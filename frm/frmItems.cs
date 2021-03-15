using SkynetDota2Mods.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SkynetDota2Mods
{
    public partial class frmItems : Form
    {
        public static frmItems frm;
        private Items Item;
        public frmItems(Items item)
        {
            InitializeComponent();
            this.Item = item;
            frm = this;

            CheckForIllegalCrossThreadCalls = false;  //Para permitir acceso a los subprocesos
            LoadItems(item.used_by_heroes, item.item_slot, item.prefab);

            Icon a;
            a = Resources.cursor;
            this.Cursor = new Cursor(a.Handle);
        }


        public void LoadStyles(Items item)
        {
            ItemContainer.Controls.Clear();

            List<Items> Styles = new List<Items>();
            for (int i = 0; i < item.styles.Count; i++)
            {
                string image_inventory = "";
                if (string.IsNullOrEmpty(item.styles[i].icon_path))
                    image_inventory = item.image_inventory;
                else
                    image_inventory = item.styles[i].icon_path;

                Items newitem = new Items()
                {
                    ItemID = item.ItemID,
                    name = item.name,
                    prefab = item.prefab,
                    style = item.styles[i],
                    image_inventory = image_inventory,
                    item_description = item.item_description,
                    item_name = item.item_name,
                    item_rarity = item.item_rarity,
                    item_slot = item.item_slot,
                    item_type_name = item.item_type_name,
                    used_by_heroes = item.used_by_heroes
                };
                Styles.Add(newitem);
            }

            int x = 5;
            int y = 5;
            for (int i = 0; i < Styles.Count; i++)
            {
                ItemContainer.Controls.Add(
                new ItemControl(this)
                {
                    Item = Styles[i],
                    Size = new System.Drawing.Size(106, 80),
                    Location = new System.Drawing.Point(x, y)
                });

                if (x > 203)
                {
                    x = 5;
                    y = y + 92;
                }
                else
                    x = x + 115;

            }

            SetSize();
        }

        private void LoadItems(string Hero, string Slot, string prefab)
        {
            try
            {
                int x = 5;
                int y = 5;
                List<Items> items = new List<Items>();

                if (prefab == "taunt")
                    items = frmMain.frm.Taunts.FindAll(i => i.used_by_heroes == Hero);
                else
                    items = frmMain.frm.items.FindAll(i => i.used_by_heroes == Hero && i.item_slot == Slot);

                items.Sort((s1, s2) => s2.ItemID.CompareTo(s1.ItemID));


                items.Add(new Items() { name = "Default item", ItemID = "0000", item_slot = Slot, prefab = prefab, used_by_heroes = Hero });

                for (int i = 0; i < items.Count; i++)
                {
                    Items item = items[i];

                    ItemContainer.Controls.Add(
                    new ItemControl(this)
                    {
                        Name = Hero,
                        Item = item,
                        Size = new System.Drawing.Size(106, 80),
                        Location = new System.Drawing.Point(x, y)
                    });

                    if (x > 203)
                    {
                        x = 5;
                        y = y + 92;
                    }
                    else
                        x = x + 115;
                }
                //Add Click event
                for (int i = 0; i < ItemContainer.Controls.Count; i++)
                {
                    ItemContainer.Controls[i].Click += new System.EventHandler(this.Item_Click);

                }
            }
            catch { }

            SetSize();
        }

        private void Item_Click(object sender, EventArgs e)
        {

        }

        private void frmMessage_Activated(object sender, EventArgs e)
        {
        }

        private void frmMessage_Deactivate(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmItems_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        }

        private void SetSize()
        {
            if (ItemContainer.Controls.Count >= 12)
            {
                this.MaximumSize = new System.Drawing.Size(364, 325);
                this.MinimumSize = new System.Drawing.Size(364, 325);
                this.Size = new System.Drawing.Size(364, 325);
            }
            else
            {
                this.MaximumSize = new System.Drawing.Size(352, 325);
                this.MinimumSize = new System.Drawing.Size(352, 325);
                this.Size = new System.Drawing.Size(352, 325);
            }
        }

        private void FrmItems_Load(object sender, EventArgs e)
        {

        }

        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            modCommon.Show("");
        }
    }
}
