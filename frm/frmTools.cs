using SkynetDota2Mods.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace SkynetDota2Mods
{
    public partial class frmTools : Form
    {
        public static frmTools frm;
        public Items item;
        public frmTools(Items items)
        {
            InitializeComponent();
            frm = this;
            CheckForIllegalCrossThreadCalls = false;  //Para permitir acceso a los subprocesos
            item = items;

            Form Mplayer = Application.OpenForms.OfType<Form>().SingleOrDefault(pre => pre.Name == "frmLoading");
            if (Mplayer != null)
                Mplayer.Close();


            LoadItems();

            Icon a;
            a = Resources.cursor;
            this.Cursor = new Cursor(a.Handle);

        }

        private void LoadItems()
        {
            try
            {
                int x = 5;
                int y = 5;
                List<Items> items = new List<Items>();

                items = ItemsManager.GetItemsFromPrefab(item);

                items.Sort((s1, s2) => s2.ItemID.CompareTo(s1.ItemID));

                for (int i = 0; i < items.Count; i++)
                {

                    Items it = items[i];

                    ItemContainer.Controls.Add(
                    new BundleBox(this)
                    {
                        item = it,
                        isTool = true,
                        Size = new System.Drawing.Size(93, 67),
                        Location = new System.Drawing.Point(x, y)
                    });

                    if (x > 220)
                    {
                        x = 5;
                        y = y + 135;
                    }
                    else
                        x = x + 128;
                }
            }
            catch {}
            SetSize();
        }
        public void LoadStyles(Items item)
        {
            ItemContainer.Controls.Clear();

            List<Items> Styles = new List<Items>();
            for (int i = 0; i < item.styles.Count; i++)
            {
                string image_inventory = item.styles[i].icon_path;

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
                    new BundleBox(this)
                    {
                        item = Styles[i],
                        isTool = true,
                        Size = new System.Drawing.Size(93, 67),
                        Location = new System.Drawing.Point(x, y)
                    });

                if (x > 220)
                {
                    x = 5;
                    y = y + 100;
                }
                else
                    x = x + 115;

            }
            SetSize();

        }

        private void SetSize()
        {
            if (ItemContainer.Controls.Count >= 12)
            {
                this.MaximumSize = new System.Drawing.Size(417, 325);
                this.MinimumSize = new System.Drawing.Size(417, 325);
                this.Size = new System.Drawing.Size(417, 325);
            }
            else
            {
                this.MaximumSize = new System.Drawing.Size(405, 325);
                this.MinimumSize = new System.Drawing.Size(405, 325);
                this.Size = new System.Drawing.Size(405, 325);
            }

        }

        private void Item_Click(object sender, EventArgs e)
        {
            modCommon.Show(((ItemControl)sender).Location.X);
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

        private void LoadWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {

        }
    }
}
