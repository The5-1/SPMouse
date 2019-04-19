using System;
using System.Drawing;
using System.Windows.Forms;

namespace SPMouse
{
    public class SPMouse : System.Windows.Forms.Form
    {
        private Icon icon;
        private NotifyIcon trayIcon;
        private Label label1;

        public SPMouse()
        {
            icon = Properties.Resources.SPM_Icon;
            trayIcon = new NotifyIcon();
            trayIcon.BalloonTipIcon = ToolTipIcon.Info;
            trayIcon.BalloonTipTitle = "Surgical Precision Mouse";
            trayIcon.BalloonTipText = "Yay!";
            this.Icon = icon; //load the icon from the project property Resources
            this.Resize += resizeCallback;
            label1 = new Label();
            label1.Text = "ASDF";
            label1.Location = new System.Drawing.Point(24, 504);
            label1.Size = new System.Drawing.Size(392, 23);


            this.Controls.AddRange(new System.Windows.Forms.Control[] {label1});
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SPMouse
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "SPMouse";
            this.Load += new System.EventHandler(this.loadCallback);
            this.ResumeLayout(false);

        }

        private void loadCallback(object sender, EventArgs e)
        {

        }

        static void Main()
        {
            Application.Run(new SPMouse());
        }


        private void resizeCallback(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                trayIcon.Visible = true;
                trayIcon.ShowBalloonTip(500);
                this.Hide();
            }

            else if (this.WindowState == FormWindowState.Normal)
            {
                trayIcon.Visible = false;
            }
        }

        private void ExitClick(Object sender, EventArgs e)
        {
            this.Dispose();
            Application.Exit();
        }

        //==========================================================================
        private void Action1Click(Object sender, EventArgs e)
        {
            // nur als Beispiel:
            // new MyForm ().Show ();
        }


    }
}