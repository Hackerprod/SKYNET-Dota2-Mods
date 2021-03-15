namespace SkynetDota2Mods
{
    partial class frmSettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.Reset = new FlatButton();
            this.ItemsToReset = new HeroItems.HeroComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.ActiveSounds = new FlatCheckBox();
            this.deleteMod = new FlatButton();
            this.label4 = new System.Windows.Forms.Label();
            this.saveBtn = new FlatButton();
            this.label3 = new System.Windows.Forms.Label();
            this.CheckCache = new FlatButton();
            this.label2 = new System.Windows.Forms.Label();
            this.Dota2Path = new XNova_Utils.Others.FlatTextBox();
            this.changeBtn = new FlatButton();
            this.label1 = new System.Windows.Forms.Label();
            this.OpenDota = new FlatCheckBox();
            this.GenerateOnStart = new FlatCheckBox();
            this.CloseBox = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.SETTINGS = new System.Windows.Forms.Label();
            this.CursorWorker = new System.ComponentModel.BackgroundWorker();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CloseBox)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(30)))), ((int)(((byte)(33)))));
            this.panel1.Controls.Add(this.Reset);
            this.panel1.Controls.Add(this.ItemsToReset);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.ActiveSounds);
            this.panel1.Controls.Add(this.deleteMod);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.saveBtn);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.CheckCache);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.Dota2Path);
            this.panel1.Controls.Add(this.changeBtn);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.OpenDota);
            this.panel1.Controls.Add(this.GenerateOnStart);
            this.panel1.Controls.Add(this.CloseBox);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel1.Location = new System.Drawing.Point(1, 1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(767, 406);
            this.panel1.TabIndex = 0;
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Event_MouseDown);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Event_MouseMove);
            this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Event_MouseUp);
            // 
            // Reset
            // 
            this.Reset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(47)))), ((int)(((byte)(48)))));
            this.Reset.BackColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(62)))), ((int)(((byte)(63)))));
            this.Reset.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Reset.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Reset.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(157)))), ((int)(((byte)(160)))));
            this.Reset.ForeColorMouseOver = System.Drawing.Color.Empty;
            this.Reset.ImageAlignment = FlatButton._ImgAlign.Left;
            this.Reset.ImageIcon = null;
            this.Reset.Location = new System.Drawing.Point(645, 271);
            this.Reset.Name = "Reset";
            this.Reset.Rounded = false;
            this.Reset.Size = new System.Drawing.Size(93, 29);
            this.Reset.Style = FlatButton._Style.TextOnly;
            this.Reset.TabIndex = 264;
            this.Reset.Text = "Reset item";
            this.Reset.Click += new System.EventHandler(this.Reset_Click);
            // 
            // ItemsToReset
            // 
            this.ItemsToReset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(47)))), ((int)(((byte)(48)))));
            this.ItemsToReset.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.ItemsToReset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ItemsToReset.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(157)))), ((int)(((byte)(160)))));
            this.ItemsToReset.FormattingEnabled = true;
            this.ItemsToReset.ItemHeight = 18;
            this.ItemsToReset.Location = new System.Drawing.Point(401, 274);
            this.ItemsToReset.Name = "ItemsToReset";
            this.ItemsToReset.Size = new System.Drawing.Size(229, 24);
            this.ItemsToReset.TabIndex = 263;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(157)))), ((int)(((byte)(160)))));
            this.label5.Location = new System.Drawing.Point(397, 248);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(233, 19);
            this.label5.TabIndex = 262;
            this.label5.Text = "Reset individual items from database";
            // 
            // ActiveSounds
            // 
            this.ActiveSounds.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(29)))), ((int)(((byte)(32)))));
            this.ActiveSounds.BoxBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(47)))), ((int)(((byte)(48)))));
            this.ActiveSounds.BoxColor = System.Drawing.Color.FromArgb(((int)(((byte)(7)))), ((int)(((byte)(164)))), ((int)(((byte)(245)))));
            this.ActiveSounds.Checked = false;
            this.ActiveSounds.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ActiveSounds.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.ActiveSounds.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(157)))), ((int)(((byte)(160)))));
            this.ActiveSounds.Location = new System.Drawing.Point(401, 189);
            this.ActiveSounds.Name = "ActiveSounds";
            this.ActiveSounds.Options = FlatCheckBox._Options.Style1;
            this.ActiveSounds.Size = new System.Drawing.Size(177, 22);
            this.ActiveSounds.TabIndex = 261;
            this.ActiveSounds.Text = "Sounds of the program";
            // 
            // deleteMod
            // 
            this.deleteMod.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(47)))), ((int)(((byte)(48)))));
            this.deleteMod.BackColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(62)))), ((int)(((byte)(63)))));
            this.deleteMod.Cursor = System.Windows.Forms.Cursors.Hand;
            this.deleteMod.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.deleteMod.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(157)))), ((int)(((byte)(160)))));
            this.deleteMod.ForeColorMouseOver = System.Drawing.Color.Empty;
            this.deleteMod.ImageAlignment = FlatButton._ImgAlign.Left;
            this.deleteMod.ImageIcon = null;
            this.deleteMod.Location = new System.Drawing.Point(14, 330);
            this.deleteMod.Name = "deleteMod";
            this.deleteMod.Rounded = false;
            this.deleteMod.Size = new System.Drawing.Size(93, 29);
            this.deleteMod.Style = FlatButton._Style.TextOnly;
            this.deleteMod.TabIndex = 260;
            this.deleteMod.Text = "Remove mod";
            this.deleteMod.Click += new System.EventHandler(this.DeleteMod_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(157)))), ((int)(((byte)(160)))));
            this.label4.Location = new System.Drawing.Point(10, 308);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(166, 19);
            this.label4.TabIndex = 259;
            this.label4.Text = "Remove Mod from Dota2";
            // 
            // saveBtn
            // 
            this.saveBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(47)))), ((int)(((byte)(48)))));
            this.saveBtn.BackColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(62)))), ((int)(((byte)(63)))));
            this.saveBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.saveBtn.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.saveBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(157)))), ((int)(((byte)(160)))));
            this.saveBtn.ForeColorMouseOver = System.Drawing.Color.Empty;
            this.saveBtn.ImageAlignment = FlatButton._ImgAlign.Left;
            this.saveBtn.ImageIcon = null;
            this.saveBtn.Location = new System.Drawing.Point(14, 270);
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Rounded = false;
            this.saveBtn.Size = new System.Drawing.Size(93, 29);
            this.saveBtn.Style = FlatButton._Style.TextOnly;
            this.saveBtn.TabIndex = 258;
            this.saveBtn.Text = "Save changes";
            this.saveBtn.Click += new System.EventHandler(this.SaveBtn_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(157)))), ((int)(((byte)(160)))));
            this.label3.Location = new System.Drawing.Point(10, 248);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(165, 19);
            this.label3.TabIndex = 257;
            this.label3.Text = "Save changes in database";
            // 
            // CheckCache
            // 
            this.CheckCache.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(47)))), ((int)(((byte)(48)))));
            this.CheckCache.BackColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(62)))), ((int)(((byte)(63)))));
            this.CheckCache.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CheckCache.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.CheckCache.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(157)))), ((int)(((byte)(160)))));
            this.CheckCache.ForeColorMouseOver = System.Drawing.Color.Empty;
            this.CheckCache.ImageAlignment = FlatButton._ImgAlign.Left;
            this.CheckCache.ImageIcon = null;
            this.CheckCache.Location = new System.Drawing.Point(14, 211);
            this.CheckCache.Name = "CheckCache";
            this.CheckCache.Rounded = false;
            this.CheckCache.Size = new System.Drawing.Size(93, 29);
            this.CheckCache.Style = FlatButton._Style.TextOnly;
            this.CheckCache.TabIndex = 256;
            this.CheckCache.Text = "Check cache";
            this.CheckCache.Click += new System.EventHandler(this.CheckCache_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(157)))), ((int)(((byte)(160)))));
            this.label2.Location = new System.Drawing.Point(10, 189);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(143, 19);
            this.label2.TabIndex = 255;
            this.label2.Text = "Check cache manually";
            // 
            // Dota2Path
            // 
            this.Dota2Path.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(47)))), ((int)(((byte)(48)))));
            this.Dota2Path.BackColorControl = System.Drawing.Color.Empty;
            this.Dota2Path.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.Dota2Path.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(157)))), ((int)(((byte)(160)))));
            this.Dota2Path.isCustomColor = false;
            this.Dota2Path.Location = new System.Drawing.Point(14, 149);
            this.Dota2Path.MaxLength = 32767;
            this.Dota2Path.Multiline = false;
            this.Dota2Path.Name = "Dota2Path";
            this.Dota2Path.OnlyNumber = false;
            this.Dota2Path.ReadOnly = false;
            this.Dota2Path.Size = new System.Drawing.Size(381, 28);
            this.Dota2Path.TabIndex = 254;
            this.Dota2Path.Text = "flatTextBox1";
            this.Dota2Path.TextAlignment = System.Windows.Forms.HorizontalAlignment.Left;
            this.Dota2Path.UseSystemPasswordChar = false;
            this.Dota2Path.TextChanged += new System.EventHandler(this.Dota2Path_TextChanged);
            // 
            // changeBtn
            // 
            this.changeBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(47)))), ((int)(((byte)(48)))));
            this.changeBtn.BackColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(62)))), ((int)(((byte)(63)))));
            this.changeBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.changeBtn.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.changeBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(157)))), ((int)(((byte)(160)))));
            this.changeBtn.ForeColorMouseOver = System.Drawing.Color.Empty;
            this.changeBtn.ImageAlignment = FlatButton._ImgAlign.Left;
            this.changeBtn.ImageIcon = null;
            this.changeBtn.Location = new System.Drawing.Point(401, 149);
            this.changeBtn.Name = "changeBtn";
            this.changeBtn.Rounded = false;
            this.changeBtn.Size = new System.Drawing.Size(93, 29);
            this.changeBtn.Style = FlatButton._Style.TextOnly;
            this.changeBtn.TabIndex = 253;
            this.changeBtn.Text = "Cambiar";
            this.changeBtn.Visible = false;
            this.changeBtn.Click += new System.EventHandler(this.ChangeBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(157)))), ((int)(((byte)(160)))));
            this.label1.Location = new System.Drawing.Point(10, 127);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(439, 19);
            this.label1.TabIndex = 252;
            this.label1.Text = "\"dota\" folder location (Commonly at common/dota 2 beta/game/dota)";
            // 
            // OpenDota
            // 
            this.OpenDota.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(29)))), ((int)(((byte)(32)))));
            this.OpenDota.BoxBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(47)))), ((int)(((byte)(48)))));
            this.OpenDota.BoxColor = System.Drawing.Color.FromArgb(((int)(((byte)(7)))), ((int)(((byte)(164)))), ((int)(((byte)(245)))));
            this.OpenDota.Checked = false;
            this.OpenDota.Cursor = System.Windows.Forms.Cursors.Hand;
            this.OpenDota.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.OpenDota.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(157)))), ((int)(((byte)(160)))));
            this.OpenDota.Location = new System.Drawing.Point(14, 87);
            this.OpenDota.Name = "OpenDota";
            this.OpenDota.Options = FlatCheckBox._Options.Style1;
            this.OpenDota.Size = new System.Drawing.Size(346, 22);
            this.OpenDota.TabIndex = 250;
            this.OpenDota.Text = "Open Dota2 when finish operation";
            // 
            // GenerateOnStart
            // 
            this.GenerateOnStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(29)))), ((int)(((byte)(32)))));
            this.GenerateOnStart.BoxBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(47)))), ((int)(((byte)(48)))));
            this.GenerateOnStart.BoxColor = System.Drawing.Color.FromArgb(((int)(((byte)(7)))), ((int)(((byte)(164)))), ((int)(((byte)(245)))));
            this.GenerateOnStart.Checked = false;
            this.GenerateOnStart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.GenerateOnStart.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.GenerateOnStart.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(157)))), ((int)(((byte)(160)))));
            this.GenerateOnStart.Location = new System.Drawing.Point(14, 59);
            this.GenerateOnStart.Name = "GenerateOnStart";
            this.GenerateOnStart.Options = FlatCheckBox._Options.Style1;
            this.GenerateOnStart.Size = new System.Drawing.Size(313, 22);
            this.GenerateOnStart.TabIndex = 249;
            this.GenerateOnStart.Text = "Check cache when the program is starting ";
            // 
            // CloseBox
            // 
            this.CloseBox.Image = global::SkynetDota2Mods.Properties.Resources.SClose1;
            this.CloseBox.Location = new System.Drawing.Point(5, 3);
            this.CloseBox.Name = "CloseBox";
            this.CloseBox.Size = new System.Drawing.Size(22, 23);
            this.CloseBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.CloseBox.TabIndex = 1;
            this.CloseBox.TabStop = false;
            this.CloseBox.Click += new System.EventHandler(this.CloseBox_Click);
            this.CloseBox.MouseLeave += new System.EventHandler(this.CloseBox_MouseLeave);
            this.CloseBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CloseBox_MouseMove);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(26)))));
            this.panel2.Controls.Add(this.SETTINGS);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(767, 30);
            this.panel2.TabIndex = 0;
            this.panel2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Event_MouseDown);
            this.panel2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Event_MouseMove);
            this.panel2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Event_MouseUp);
            // 
            // SETTINGS
            // 
            this.SETTINGS.AutoSize = true;
            this.SETTINGS.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SETTINGS.ForeColor = System.Drawing.Color.Gainsboro;
            this.SETTINGS.Location = new System.Drawing.Point(37, 8);
            this.SETTINGS.Name = "SETTINGS";
            this.SETTINGS.Size = new System.Drawing.Size(71, 13);
            this.SETTINGS.TabIndex = 0;
            this.SETTINGS.Text = "SETTINGS";
            this.SETTINGS.Click += new System.EventHandler(this.SETTINGS_Click);
            this.SETTINGS.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Event_MouseDown);
            this.SETTINGS.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Event_MouseMove);
            this.SETTINGS.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Event_MouseUp);
            // 
            // CursorWorker
            // 
            this.CursorWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.CursorWorker_DoWork);
            // 
            // frmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(44)))));
            this.ClientSize = new System.Drawing.Size(769, 408);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmSettings";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmSettings";
            this.Deactivate += new System.EventHandler(this.FrmSettings_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmSettings_FormClosing);
            this.Load += new System.EventHandler(this.FrmSettings_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CloseBox)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label SETTINGS;
        private System.Windows.Forms.PictureBox CloseBox;
        private FlatCheckBox GenerateOnStart;
        private FlatCheckBox OpenDota;
        private System.Windows.Forms.Label label1;
        private FlatButton changeBtn;
        private XNova_Utils.Others.FlatTextBox Dota2Path;
        private FlatButton CheckCache;
        private System.Windows.Forms.Label label2;
        private System.ComponentModel.BackgroundWorker CursorWorker;
        private FlatButton saveBtn;
        private System.Windows.Forms.Label label3;
        private FlatButton deleteMod;
        private System.Windows.Forms.Label label4;
        private FlatCheckBox ActiveSounds;
        private System.Windows.Forms.Label label5;
        private FlatButton Reset;
        private HeroItems.HeroComboBox ItemsToReset;
    }
}