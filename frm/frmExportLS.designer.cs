partial class frmExportLS
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmExportLS));
            this.acceptBtn = new System.Windows.Forms.Button();
            this.ok = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtMessage = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.LoadScreen = new System.Windows.Forms.PictureBox();
            this.cancelBtn = new FlatButton();
            this.ExportWorker = new System.ComponentModel.BackgroundWorker();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LoadScreen)).BeginInit();
            this.SuspendLayout();
            // 
            // acceptBtn
            // 
            this.acceptBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.acceptBtn.Location = new System.Drawing.Point(485, 375);
            this.acceptBtn.Name = "acceptBtn";
            this.acceptBtn.Size = new System.Drawing.Size(75, 23);
            this.acceptBtn.TabIndex = 16;
            this.acceptBtn.Text = "button1";
            this.acceptBtn.UseVisualStyleBackColor = true;
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
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(29)))), ((int)(((byte)(32)))));
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.txtMessage);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.cancelBtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(1, 1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(400, 308);
            this.panel1.TabIndex = 25;
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Event_MouseDown);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Event_MouseMove);
            this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Event_MouseUp);
            // 
            // panel2
            // 
            this.panel2.Location = new System.Drawing.Point(380, 246);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(19, 20);
            this.panel2.TabIndex = 30;
            // 
            // txtMessage
            // 
            this.txtMessage.AutoSize = true;
            this.txtMessage.ForeColor = System.Drawing.Color.Gray;
            this.txtMessage.Location = new System.Drawing.Point(6, 245);
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(0, 16);
            this.txtMessage.TabIndex = 29;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.LoadScreen);
            this.panel3.Location = new System.Drawing.Point(7, 8);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(2);
            this.panel3.Size = new System.Drawing.Size(386, 234);
            this.panel3.TabIndex = 28;
            // 
            // LoadScreen
            // 
            this.LoadScreen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LoadScreen.Location = new System.Drawing.Point(2, 2);
            this.LoadScreen.Name = "LoadScreen";
            this.LoadScreen.Size = new System.Drawing.Size(382, 230);
            this.LoadScreen.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.LoadScreen.TabIndex = 27;
            this.LoadScreen.TabStop = false;
            // 
            // cancelBtn
            // 
            this.cancelBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(47)))), ((int)(((byte)(48)))));
            this.cancelBtn.BackColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(62)))), ((int)(((byte)(63)))));
            this.cancelBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cancelBtn.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cancelBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(157)))), ((int)(((byte)(160)))));
            this.cancelBtn.ForeColorMouseOver = System.Drawing.Color.Empty;
            this.cancelBtn.ImageAlignment = FlatButton._ImgAlign.Left;
            this.cancelBtn.ImageIcon = null;
            this.cancelBtn.Location = new System.Drawing.Point(148, 276);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Rounded = false;
            this.cancelBtn.Size = new System.Drawing.Size(93, 29);
            this.cancelBtn.Style = FlatButton._Style.TextOnly;
            this.cancelBtn.TabIndex = 26;
            this.cancelBtn.Text = "Cancelar";
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // ExportWorker
            // 
            this.ExportWorker.WorkerSupportsCancellation = true;
            this.ExportWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.ExportWorker_DoWork);
            this.ExportWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.ExportWorker_RunWorkerCompleted);
            // 
            // frmExportLS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
        this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(7)))), ((int)(((byte)(164)))), ((int)(((byte)(245)))));
            this.ClientSize = new System.Drawing.Size(402, 310);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.ok);
            this.Controls.Add(this.acceptBtn);
            this.Font = new System.Drawing.Font("Segoe UI Emoji", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frmExportLS";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Message";
            this.Shown += new System.EventHandler(this.FrmExportLS_Shown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.LoadScreen)).EndInit();
            this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.Button acceptBtn;
    private System.Windows.Forms.Button ok;
    private System.Windows.Forms.Panel panel1;
    private FlatButton cancelBtn;
    private System.Windows.Forms.PictureBox LoadScreen;
    private System.Windows.Forms.Panel panel3;
    private System.Windows.Forms.Label txtMessage;
    private System.ComponentModel.BackgroundWorker ExportWorker;
    private System.Windows.Forms.Panel panel2;
}
