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
    public class BundleBox : UserControl
    {
        private IContainer components;
        private PictureBox Icon;
        private Panel panel1;
        private Label name;
        private Label rarity;
        private Label label1;
        private Label Styles;
        private ToolTip toolTip1;
        frmTools Tools;
        public BundleBox()
        {
            this.InitializeComponent();
            this.MaximumSize = new System.Drawing.Size(128, 124);
            this.MinimumSize = new System.Drawing.Size(128, 124);
            this.Size = new System.Drawing.Size(128, 124);

        }
        public BundleBox(frmTools frmtools)
        {
            this.InitializeComponent();
            Tools = frmtools;
        }
        private bool _Static { get; set; }
        public bool Static { get { return _Static; } set { _Static = value; } }
        private Items _Item { get; set; }
        public Items item
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
                    name.Text = _Item.name;

                    if (!_Static)
                    {
                        if (_Item.styles.Count > 0)
                            Styles.Text = _Item.styles.Count.ToString() + " STYLES";                    
                    }


                    rarity.Text = modCommon.FirstUpper(_Item.item_rarity);
                    rarity.ForeColor = modCommon.ColorBundleFromRarity(_Item.ItemID);
                    panel1.BackColor = modCommon.ColorBundleFromRarity(_Item.ItemID);
                    Icon.Image = ItemsManager.GetImage(_Item.image_inventory);

                }
                catch
                {
                    Icon.Image = modCommon.DefaultIcon;
                }
            }
        }
        private bool _ShowToolTip;
        public bool ShowToolTip
        {
            get { return _ShowToolTip; }
            set { _ShowToolTip = value; }
        }

        public void Reset()
        {
            if (item == null)
            {
                item = new Items();
            }
            else
            {
                string prefab = item.prefab;
                item = null;
                item = new Items();
                item.prefab = prefab;
            }
            name.Text = "Reset by user";
            rarity.Text = "";
            Icon.Image = Resources.default_item;
        }

        public string ToDelete = "Items";
        public bool isTool = false;

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
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Styles = new System.Windows.Forms.Label();
            this.Icon = new System.Windows.Forms.PictureBox();
            this.name = new System.Windows.Forms.Label();
            this.rarity = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Icon)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.Styles);
            this.panel1.Controls.Add(this.Icon);
            this.panel1.Location = new System.Drawing.Point(3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(1);
            this.panel1.Size = new System.Drawing.Size(122, 85);
            this.panel1.TabIndex = 2;
            // 
            // Styles
            // 
            this.Styles.AutoSize = true;
            this.Styles.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(42)))), ((int)(((byte)(45)))));
            this.Styles.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold);
            this.Styles.ForeColor = System.Drawing.Color.White;
            this.Styles.Location = new System.Drawing.Point(3, 50);
            this.Styles.Name = "Styles";
            this.Styles.Size = new System.Drawing.Size(0, 12);
            this.Styles.TabIndex = 5;
            this.Styles.Click += new System.EventHandler(this.Item_Click);
            // 
            // Icon
            // 
            this.Icon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Icon.Location = new System.Drawing.Point(1, 1);
            this.Icon.Name = "Icon";
            this.Icon.Size = new System.Drawing.Size(120, 83);
            this.Icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Icon.TabIndex = 0;
            this.Icon.TabStop = false;
            this.Icon.Click += new System.EventHandler(this.Item_Click);
            this.Icon.MouseLeave += new System.EventHandler(this.Icon_MouseLeave);
            this.Icon.MouseHover += new System.EventHandler(this.Icon_MouseHover);
            // 
            // name
            // 
            this.name.AutoSize = true;
            this.name.Font = new System.Drawing.Font("Segoe UI Emoji", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.name.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.name.Location = new System.Drawing.Point(0, 91);
            this.name.MaximumSize = new System.Drawing.Size(110, 16);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(70, 15);
            this.name.TabIndex = 2;
            this.name.Text = "Default item";
            this.name.Click += new System.EventHandler(this.Item_Click);
            // 
            // rarity
            // 
            this.rarity.AutoSize = true;
            this.rarity.Font = new System.Drawing.Font("Segoe UI Emoji", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rarity.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.rarity.Location = new System.Drawing.Point(35, 105);
            this.rarity.Margin = new System.Windows.Forms.Padding(0);
            this.rarity.Name = "rarity";
            this.rarity.Size = new System.Drawing.Size(0, 15);
            this.rarity.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Emoji", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.label1.Location = new System.Drawing.Point(1, 105);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "Rareza:";
            // 
            // BundleBox
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(48)))), ((int)(((byte)(51)))));
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rarity);
            this.Controls.Add(this.name);
            this.Controls.Add(this.panel1);
            this.MaximumSize = new System.Drawing.Size(128, 124);
            this.MinimumSize = new System.Drawing.Size(128, 124);
            this.Name = "BundleBox";
            this.Size = new System.Drawing.Size(128, 124);
            this.Load += new System.EventHandler(this.BoxTool_Load);
            this.Click += new System.EventHandler(this.Item_Click);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Icon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void BoxTool_Load(object sender, EventArgs e)
        {
            //Styles.Parent = Icon;
        }


        private void Item_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Styles.Text))
            {
                frmMain.frm.Bundle_Click(this);
                frmTools.frm?.Close();
            }
            else
            {
                Tools.LoadStyles(item);
            }
        }

        private void Icon_MouseHover(object sender, EventArgs e)
        {
            if (ShowToolTip)
                frmMain.SetBoxInfo(this);
        }

        private void Icon_MouseLeave(object sender, EventArgs e)
        {
            frmMain.frm.boxInfo.Visible = false;
        }

    }

}
