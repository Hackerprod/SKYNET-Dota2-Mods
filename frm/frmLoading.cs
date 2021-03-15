using SkynetDota2Mods.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SkynetDota2Mods
{
    public partial class frmLoading : Form
    {
        Items items;
        public frmLoading(Items item)
        {
            InitializeComponent();
            items = item;
        }

        private void CloseBox_Click(object sender, EventArgs e)
        {
            Close();
        }



        private void FrmSettings_Load(object sender, EventArgs e)
        {

        }

        private void CursorWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                for (int i = 0; i < Controls.Count; i++)
                {
                    try
                    {
                        Icon a;
                        a = Properties.Resources.cursor;
                        Controls[i].Cursor = new Cursor(a.Handle);
                        this.Cursor = new Cursor(a.Handle);
                    }
                    catch { }
                }
            }
        }

        private void FrmLoading_Shown(object sender, EventArgs e)
        {
            frmTools frmtools = new frmTools(items);
            frmtools.ShowDialog();
        }
    }
}
