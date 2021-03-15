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
    public class BoxItems : UserControl
    {
        private IContainer components;
        private Label name;
        private Panel panel2;
        private Panel panel3;
        private Panel panel1;
        private PictureBox Icon;
        private Panel panelRarity;
        public string HostName;

        public BoxItems()
        {
            this.InitializeComponent();
            this.MaximumSize = new System.Drawing.Size(93, 76);
            this.MinimumSize = new System.Drawing.Size(93, 76);
            this.Size = new System.Drawing.Size(93, 76);

        }

        public string Hero { get; set; }
        private string _Type { get; set; }
        public string Type
        {
            get
            {
                return _Type;
            }
            set
            {
                _Type = value;
                name.Text = value;
                if (string.IsNullOrEmpty(_Item?.ItemID))
                {
                    if (value == "taunt")
                        Icon.Image = Resources.default_taunt;
                    else
                        Icon.Image = ItemsManager.GetDefaultImageItem(value, Hero);
                }
            }
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
                try
                {
                    bool founded = false;
                    for (int h = 0; h < frmMain.manager.Heroes.Count; h++)
                    {
                        if (frmMain.manager.Heroes[h].Name == _Item.used_by_heroes)
                        {

                            //Getting Bundles
                            for (int i = 0; i < frmMain.manager.Heroes[h].Bundles.Count; i++)
                            {
                                if (frmMain.manager.Heroes[h].Bundles[i].item_slot == _Item.item_slot)
                                {
                                    if (_Item.ItemID == "0000")
                                    {

                                        //Delete Item for set default item
                                        frmMain.manager.Heroes[h].Bundles.Remove(frmMain.manager.Heroes[h].Bundles[i]);
                                        founded = true;
                                    }
                                    else
                                    {
                                        frmMain.manager.Heroes[h].Bundles[i] = _Item;
                                        founded = true;
                                    }
                                }
                            }


                            if (!founded)
                            frmMain.manager.Heroes[h].Bundles.Add(_Item);
                        }
                    }

                    if (_Item.ItemID == "0000")
                        Icon.Image = ItemsManager.GetDefaultImageItem(_Item.item_slot, _Item.used_by_heroes);
                    else
                    {
                        Icon.Image = ItemsManager.GetImage(_Item.image_inventory);
                        panelRarity.BackColor = modCommon.ColorItemsFromRarity(_Item.ItemID);
                    }
                }
                catch (Exception)
                {
                    //Icon.Image = Resources.testitem_slot_empty;
                }

            }
        }

        public string ToDelete = "Items";

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
            this.name = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panelRarity = new System.Windows.Forms.Panel();
            this.Icon = new System.Windows.Forms.PictureBox();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Icon)).BeginInit();
            this.SuspendLayout();
            // 
            // name
            // 
            this.name.AutoSize = true;
            this.name.Font = new System.Drawing.Font("Segoe UI Symbol", 7F, System.Drawing.FontStyle.Bold);
            this.name.ForeColor = System.Drawing.Color.Silver;
            this.name.Location = new System.Drawing.Point(0, 3);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(35, 12);
            this.name.TabIndex = 1;
            this.name.Text = "Name";
            this.name.Click += new System.EventHandler(this.Item_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.name);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(1);
            this.panel2.Size = new System.Drawing.Size(114, 19);
            this.panel2.TabIndex = 3;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.panel1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 19);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(2);
            this.panel3.Size = new System.Drawing.Size(114, 77);
            this.panel3.TabIndex = 6;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(32)))), ((int)(((byte)(35)))));
            this.panel1.Controls.Add(this.Icon);
            this.panel1.Controls.Add(this.panelRarity);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(2, 2);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.panel1.Size = new System.Drawing.Size(110, 73);
            this.panel1.TabIndex = 2;
            // 
            // panelRarity
            // 
            this.panelRarity.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelRarity.Location = new System.Drawing.Point(0, 70);
            this.panelRarity.Name = "panelRarity";
            this.panelRarity.Size = new System.Drawing.Size(110, 1);
            this.panelRarity.TabIndex = 1;
            // 
            // Icon
            // 
            this.Icon.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(32)))), ((int)(((byte)(35)))));
            this.Icon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Icon.Location = new System.Drawing.Point(0, 0);
            this.Icon.Name = "Icon";
            this.Icon.Size = new System.Drawing.Size(110, 70);
            this.Icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.Icon.TabIndex = 2;
            this.Icon.TabStop = false;
            this.Icon.Click += new System.EventHandler(this.Item_Click);
            // 
            // BoxItems
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(32)))), ((int)(((byte)(35)))));
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Name = "BoxItems";
            this.Size = new System.Drawing.Size(114, 96);
            this.Load += new System.EventHandler(this.BoxTool_Load);
            this.Click += new System.EventHandler(this.Item_Click);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Icon)).EndInit();
            this.ResumeLayout(false);

        }

        private void BoxTool_Load(object sender, EventArgs e)
        {
        }

        private void Item_Click(object sender, EventArgs e)
        {
            frmItems item = new frmItems(Item);
            DialogResult result = item.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.Item = modCommon.ItemIDSelected;
            }
        }


    }

}
