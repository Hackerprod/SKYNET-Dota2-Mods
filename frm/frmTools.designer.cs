namespace SkynetDota2Mods
{
    partial class frmTools
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
            this.acceptBtn = new System.Windows.Forms.Button();
            this.ok = new System.Windows.Forms.Button();
            this.loadWorker = new System.ComponentModel.BackgroundWorker();
            this.ItemContainer = new MetroPanel();
            this.SuspendLayout();
            // 
            // acceptBtn
            // 
            this.acceptBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(48)))), ((int)(((byte)(51)))));
            this.acceptBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.acceptBtn.Location = new System.Drawing.Point(485, 375);
            this.acceptBtn.Name = "acceptBtn";
            this.acceptBtn.Size = new System.Drawing.Size(75, 23);
            this.acceptBtn.TabIndex = 16;
            this.acceptBtn.Text = "button1";
            this.acceptBtn.UseVisualStyleBackColor = false;
            // 
            // ok
            // 
            this.ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ok.Location = new System.Drawing.Point(483, 145);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(18, 23);
            this.ok.TabIndex = 24;
            this.ok.Text = "ok";
            this.ok.UseVisualStyleBackColor = true;
            // 
            // loadWorker
            // 
            this.loadWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.LoadWorker_DoWork);
            // 
            // ItemContainer
            // 
            this.ItemContainer.AutoScroll = true;
            this.ItemContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ItemContainer.HorizontalScrollbar = true;
            this.ItemContainer.HorizontalScrollbarBarColor = true;
            this.ItemContainer.HorizontalScrollbarHighlightOnWheel = false;
            this.ItemContainer.HorizontalScrollbarSize = 10;
            this.ItemContainer.Location = new System.Drawing.Point(1, 1);
            this.ItemContainer.Name = "ItemContainer";
            this.ItemContainer.Size = new System.Drawing.Size(405, 321);
            this.ItemContainer.TabIndex = 25;
            this.ItemContainer.UseSelectable = false;
            this.ItemContainer.VerticalScrollbar = true;
            this.ItemContainer.VerticalScrollbarBarColor = true;
            this.ItemContainer.VerticalScrollbarHighlightOnWheel = false;
            this.ItemContainer.VerticalScrollbarSize = 10;
            // 
            // frmTools
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(48)))), ((int)(((byte)(51)))));
            this.ClientSize = new System.Drawing.Size(407, 323);
            this.Controls.Add(this.ItemContainer);
            this.Controls.Add(this.ok);
            this.Controls.Add(this.acceptBtn);
            this.Font = new System.Drawing.Font("Segoe UI Emoji", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frmTools";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Message";
            this.Activated += new System.EventHandler(this.frmMessage_Activated);
            this.Deactivate += new System.EventHandler(this.frmMessage_Deactivate);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmItems_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button acceptBtn;
        public  System.Windows.Forms.Button ok;
        private System.ComponentModel.BackgroundWorker loadWorker;
        private MetroPanel ItemContainer;
    }
}