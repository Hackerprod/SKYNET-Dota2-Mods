using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;
using SkynetDota2Mods;
using SkynetDota2Mods.Properties;

namespace SkynetDota2Mods
{
    public class BoxInfo : UserControl
    {
        private IContainer components;
        private Panel Content;
        private Panel panel1;
        private Panel RarityPanel;
        public Label item_name;
        private Panel panel3;
        private PictureBox HeroAvatar;
        private Panel panel4;
        private Label labelRarity;
        public Label Type;
        public Label item_slot;
        public Label item_rarity;
        private Panel panel2;
        public TextBox item_description;
        frmTools Tools;
        public BoxInfo()
        {
            this.InitializeComponent();


        }
        public BoxInfo(frmTools frmtools)
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
                    string espacio = "";
                    if (_Item.item_slot != null)
                    {
                        espacio = _Item.item_slot;
                        Type.Text = "Espacio:";

                    }
                    else if (_Item.prefab != null)
                    {
                        espacio = modCommon.PreparePrefab(_Item.prefab);
                        Type.Text = "Tipo:";
                    }
                    item_description.Text = "";
                    labelRarity.Text = "Rareza:";

                    string ItemName = ItemsManager.GetDescription(item.item_name);
                    if (ItemName.StartsWith("#"))
                        ItemName = _Item.name;
                    else
                        ItemName = ProcessName(ItemName);
                    item_name.Text = ItemName; 

                    item_rarity.Text = modCommon.FirstUpper(_Item.item_rarity);
                    item_rarity.ForeColor = modCommon.ColorBundleFromRarity(_Item.ItemID);
                    RarityPanel.BackColor = modCommon.ColorBundleFromRarity(_Item.ItemID);

                    if (string.IsNullOrEmpty(_Item.used_by_heroes))
                        HeroAvatar.Image = ItemsManager.GetImage(_Item.image_inventory);
                    else
                        HeroAvatar.Image = ItemsManager.GetImage("heroes/" + _Item.used_by_heroes);

                    item_slot.Text = espacio;

                    item_description.Text = modCommon.ConvertHtmlTotext(ItemsManager.GetDescription(_Item.item_description.ToLower()));
                    SetVisible = true;

                }
                catch
                {
                    labelRarity.Text = "";
                    Type.Text = "";
                    item_name.Text = "Sin descripción";
                    item_rarity.Text = "";
                    RarityPanel.BackColor = Color.Red;
                    HeroAvatar.Image = Resources.default_item;
                    item_slot.Text = "";
                    item_description.Text = "";
                    SetVisible = false;
                }

            }
        }
        private static string HtmlToPlainText(string html)
        {
            const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";//matches one or more (white space or line breaks) between '>' and '<'
            const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
            const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
            var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
            var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
            var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);

            var text = html;
            //Decode html specific characters
            text = System.Net.WebUtility.HtmlDecode(text);
            //Remove tag whitespace/line breaks
            text = tagWhiteSpaceRegex.Replace(text, "><");
            //Replace <br /> with line breaks
            text = lineBreakRegex.Replace(text, Environment.NewLine);
            //Strip formatting
            text = stripFormattingRegex.Replace(text, string.Empty);

            return text;
        }
        public bool SetVisible { get; private set; }

        private string ProcessName(string itemName)
        {
            string name = "";
            int wordscount = 0;
            string[] words = itemName.Split(' ');
            bool add = true;
            foreach (string word in words)
            {
                if (!add)
                    name += word + " ";

                if (wordscount + word.Length < 25 && add)
                {
                    name += word + " ";
                    wordscount = wordscount + word.Length;
                }
                else
                {
                    if (add)
                        name += Environment.NewLine + word + " ";

                    add = false;
                }

            }
            return name;
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
            this.Content = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.item_slot = new System.Windows.Forms.Label();
            this.item_rarity = new System.Windows.Forms.Label();
            this.Type = new System.Windows.Forms.Label();
            this.labelRarity = new System.Windows.Forms.Label();
            this.HeroAvatar = new System.Windows.Forms.PictureBox();
            this.RarityPanel = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.item_description = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.item_name = new System.Windows.Forms.Label();
            this.Content.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HeroAvatar)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Content
            // 
            this.Content.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(48)))), ((int)(((byte)(51)))));
            this.Content.Controls.Add(this.panel4);
            this.Content.Controls.Add(this.panel3);
            this.Content.Controls.Add(this.RarityPanel);
            this.Content.Controls.Add(this.panel2);
            this.Content.Controls.Add(this.panel1);
            this.Content.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Content.Location = new System.Drawing.Point(1, 1);
            this.Content.Name = "Content";
            this.Content.Size = new System.Drawing.Size(147, 160);
            this.Content.TabIndex = 0;
            this.Content.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Content_MouseMove);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.Black;
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 63);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(147, 2);
            this.panel4.TabIndex = 5;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.item_slot);
            this.panel3.Controls.Add(this.item_rarity);
            this.panel3.Controls.Add(this.Type);
            this.panel3.Controls.Add(this.labelRarity);
            this.panel3.Controls.Add(this.HeroAvatar);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 35);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(147, 28);
            this.panel3.TabIndex = 4;
            // 
            // item_slot
            // 
            this.item_slot.AutoSize = true;
            this.item_slot.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.item_slot.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(223)))), ((int)(((byte)(222)))));
            this.item_slot.Location = new System.Drawing.Point(90, 13);
            this.item_slot.Name = "item_slot";
            this.item_slot.Size = new System.Drawing.Size(27, 12);
            this.item_slot.TabIndex = 8;
            this.item_slot.Text = "Head";
            // 
            // item_rarity
            // 
            this.item_rarity.AutoSize = true;
            this.item_rarity.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.item_rarity.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(223)))), ((int)(((byte)(222)))));
            this.item_rarity.Location = new System.Drawing.Point(90, 1);
            this.item_rarity.Name = "item_rarity";
            this.item_rarity.Size = new System.Drawing.Size(39, 12);
            this.item_rarity.TabIndex = 7;
            this.item_rarity.Text = "Inmortal";
            // 
            // Type
            // 
            this.Type.AutoSize = true;
            this.Type.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Type.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(223)))), ((int)(((byte)(222)))));
            this.Type.Location = new System.Drawing.Point(49, 13);
            this.Type.Name = "Type";
            this.Type.Size = new System.Drawing.Size(41, 12);
            this.Type.TabIndex = 6;
            this.Type.Text = "Espacio:";
            // 
            // labelRarity
            // 
            this.labelRarity.AutoSize = true;
            this.labelRarity.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRarity.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(223)))), ((int)(((byte)(222)))));
            this.labelRarity.Location = new System.Drawing.Point(49, 1);
            this.labelRarity.Name = "labelRarity";
            this.labelRarity.Size = new System.Drawing.Size(38, 12);
            this.labelRarity.TabIndex = 5;
            this.labelRarity.Text = "Rareza:";
            // 
            // HeroAvatar
            // 
            this.HeroAvatar.Dock = System.Windows.Forms.DockStyle.Left;
            this.HeroAvatar.Location = new System.Drawing.Point(0, 0);
            this.HeroAvatar.Name = "HeroAvatar";
            this.HeroAvatar.Size = new System.Drawing.Size(46, 28);
            this.HeroAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.HeroAvatar.TabIndex = 0;
            this.HeroAvatar.TabStop = false;
            // 
            // RarityPanel
            // 
            this.RarityPanel.BackColor = System.Drawing.Color.Lime;
            this.RarityPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.RarityPanel.Location = new System.Drawing.Point(0, 33);
            this.RarityPanel.Name = "RarityPanel";
            this.RarityPanel.Size = new System.Drawing.Size(147, 2);
            this.RarityPanel.TabIndex = 3;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.item_description);
            this.panel2.Location = new System.Drawing.Point(0, 65);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(3);
            this.panel2.Size = new System.Drawing.Size(147, 92);
            this.panel2.TabIndex = 2;
            // 
            // item_description
            // 
            this.item_description.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(48)))), ((int)(((byte)(51)))));
            this.item_description.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.item_description.Dock = System.Windows.Forms.DockStyle.Fill;
            this.item_description.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F);
            this.item_description.ForeColor = System.Drawing.Color.Gray;
            this.item_description.Location = new System.Drawing.Point(3, 3);
            this.item_description.Multiline = true;
            this.item_description.Name = "item_description";
            this.item_description.Size = new System.Drawing.Size(141, 86);
            this.item_description.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.item_name);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(147, 33);
            this.panel1.TabIndex = 1;
            // 
            // item_name
            // 
            this.item_name.AutoSize = true;
            this.item_name.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(223)))), ((int)(((byte)(222)))));
            this.item_name.Location = new System.Drawing.Point(3, 4);
            this.item_name.Name = "item_name";
            this.item_name.Size = new System.Drawing.Size(125, 13);
            this.item_name.TabIndex = 4;
            this.item_name.Text = "Manifold Paradox Bundle";
            // 
            // BoxInfo
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.Content);
            this.Name = "BoxInfo";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Size = new System.Drawing.Size(149, 162);
            this.Load += new System.EventHandler(this.BoxTool_Load);
            this.Content.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HeroAvatar)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        private void BoxTool_Load(object sender, EventArgs e)
        {
        }

        private void Content_MouseMove(object sender, MouseEventArgs e)
        {
            Visible = false;
        }
    }

}
