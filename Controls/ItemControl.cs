using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;
using SkynetDota2Mods;
using SkynetDota2Mods.Properties;

namespace SkynetDota2Mods
{
    public class ItemControl : UserControl
    {
        private IContainer components;
        private PictureBox Icon;
        private Label Styles;
        frmItems frmitems; 
        public ItemControl(frmItems frm)
        {
            this.InitializeComponent();
            frmitems = frm;
        }
        private Items _Item { get; set; }
        public Items Item
        {
            get
            {
                return _Item;
            }
            set
            {
                _Item = value;
            }
        }

        [DebuggerNonUserCode]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!disposing || this.components == null)
                    return;
                this.components.Dispose();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        [DebuggerStepThrough]
        private void InitializeComponent()
        {
            this.Icon = new System.Windows.Forms.PictureBox();
            this.Styles = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.Icon)).BeginInit();
            this.SuspendLayout();
            // 
            // Icon
            // 
            this.Icon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Icon.Location = new System.Drawing.Point(1, 1);
            this.Icon.Name = "Icon";
            this.Icon.Size = new System.Drawing.Size(91, 65);
            this.Icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Icon.TabIndex = 1;
            this.Icon.TabStop = false;
            this.Icon.Click += new System.EventHandler(this.Icon_Click);
            this.Icon.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Icon_MouseMove);
            // 
            // Styles
            // 
            this.Styles.AutoSize = true;
            this.Styles.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.Styles.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold);
            this.Styles.ForeColor = System.Drawing.Color.White;
            this.Styles.Location = new System.Drawing.Point(3, 52);
            this.Styles.Name = "Styles";
            this.Styles.Size = new System.Drawing.Size(0, 12);
            this.Styles.TabIndex = 2;
            this.Styles.Click += new System.EventHandler(this.Icon_Click);
            // 
            // ItemControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(32)))), ((int)(((byte)(35)))));
            this.Controls.Add(this.Styles);
            this.Controls.Add(this.Icon);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ItemControl";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Size = new System.Drawing.Size(93, 67);
            this.Load += new System.EventHandler(this.BoxTool_Load);
            this.Click += new System.EventHandler(this.Item_Click);
            ((System.ComponentModel.ISupportInitialize)(this.Icon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private void BoxTool_Load(object sender, EventArgs e)
        {
            Styles.Parent = Icon;
            try
            {
                if (Item.styles.Count > 0)
                    Styles.Text = Item.styles.Count.ToString() + " STYLES";

                if (Item.ItemID == "0000")
                {
                    Icon.Image = ItemsManager.GetDefaultImageItem(Item.item_slot, Item.used_by_heroes);
                }
                else
                {
                    BackColor = modCommon.ColorItemsFromRarity(Item.ItemID);
                    Icon.Image = ItemsManager.GetImage(Item.image_inventory);
                }
            }
            catch (Exception)
            {
                Icon.Image = Resources.default_item;
               // modCommon.Show("data/panorama/images/" + item.image_inventory + ".png");
            }
        }

        private void Item_Click(object sender, EventArgs e)
        {

        }

        private void Icon_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Styles.Text))
            {
                modCommon.ItemIDSelected = this.Item;
                try { frmitems.ok.PerformClick(); } catch { }
            }
            else
            {
                frmitems.LoadStyles(this.Item);
            }
        }

        private void Icon_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                //frmMain.frm.WriteLine("Name: " + Item.name + ",  " + "Rareza: ", modCommon.FirstUpper(Item.item_rarity));
            }
            catch 
            { }
        }
    }
}
