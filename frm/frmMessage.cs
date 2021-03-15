using System;
using System.Drawing;
using System.Windows.Forms;

namespace XNova_Utils
{
    public partial class frmMessage : Form
    {
        private bool mouseDown;     //Mover ventana
        private Point lastLocation; //Mover ventana
        public TypeMessage typeMessage;
        public frmMessage(string message, TypeMessage type = TypeMessage.Normal)
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;  //Para permitir acceso a los subprocesos

            typeMessage = type;

            switch (typeMessage)
            {
                case TypeMessage.Alert:

                    break;
                case TypeMessage.Normal:
                    acepctBtn.Visible = false;
                    cancelBtn.Text = "Cerrar";
                    break;
                case TypeMessage.YesNo:

                    break;
            }
            txtMessage.Text = message;
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


        private void cancelBtn_Click(object sender, EventArgs e)
        {
            Cancel.PerformClick();
            Close();
        }

        private void acepctBtn_Click(object sender, EventArgs e)
        {
            ok.PerformClick();
        }

        public enum TypeMessage
        {
            Alert,
            Normal,
            YesNo
        }

        private void frmMessage_Activated(object sender, EventArgs e)
        {
        }

        private void frmMessage_Deactivate(object sender, EventArgs e)
        {
        }
    }
}
