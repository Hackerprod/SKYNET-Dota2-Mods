using System;
using System.Drawing;
using System.Windows.Forms;

namespace XNova_Utils
{
    public partial class frmAbout : Form
    {
        private bool mouseDown;     //Mover ventana
        private Point lastLocation; //Mover ventana
        public TypeMessage typeMessage;
        public frmAbout()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;  //Para permitir acceso a los subprocesos
            textBox1.Focus();

            VersionInfo.Text = "v1.9";

            //AudioPlayer.PlaySound("sounds/ui/projection/crystal_maiden_pick.vsnd", false);
            //AudioPlayer.PlaySound("sounds/ui/projection/enigma_pick.vsnd", false);
            //AudioPlayer.PlaySound("sounds/ui/projection/disruptor_pick.vsnd", false);
            //AudioPlayer.PlaySound("sounds/ui/projection/dark_seer_pick.vsnd", false);
            AudioPlayer.PlaySound("sounds/ui/projection/magantaur_pick.vsnd", false);

            //AudioPlayer.PlaySound("sounds/ui/projection/doom_pick.vsnd", false);
            //AudioPlayer.PlaySound("sounds/ui/projection/juggernaut_pick.vsnd", false);
            //AudioPlayer.PlaySound("sounds/ui/projection/kotl_pick.vsnd", false);
            //AudioPlayer.PlaySound("sounds/ui/projection/legion_commander_pick.vsnd", false);
            //AudioPlayer.PlaySound("sounds/ui/projection/lina_pick.vsnd", false);
            //AudioPlayer.PlaySound("sounds/ui/projection/naga_siren_pick.vsnd", false);
            //AudioPlayer.PlaySound("sounds/ui/projection/chen_pick.vsnd", false);
            //AudioPlayer.PlaySound("sounds/ui/projection/pudge_pick.vsnd", false);
            //AudioPlayer.PlaySound("sounds/ui/projection/tinker_pick.vsnd", false);
            //AudioPlayer.PlaySound("sounds/ui/projection/witch_doctor_pick.vsnd", false);
        }
        private void Event_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                Location = new Point((Location.X - lastLocation.X) + e.X, (Location.Y - lastLocation.Y) + e.Y);
                Update();
                Opacity = 0.93;
            }
        }

        private void Event_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;

        }

        private void Event_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
            Opacity = 100;
        }


        public enum TypeMessage
        {
            Alert,
            Normal,
            YesNo
        }

        private void frmMessage_Activated(object sender, EventArgs e)
        {
            textBox1.Focus();
        }

        private void frmMessage_Deactivate(object sender, EventArgs e)
        {
            Close();
        }

        private void FrmAbout_Load(object sender, EventArgs e)
        {
            pictureBox1.Focus();
        }


        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            Close();
        }
    }
}
