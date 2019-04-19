using System;
using System.Drawing;
using System.Windows.Forms;

namespace SPMouse
{
    public class SPMouse : System.Windows.Forms.Form
    {
        private NotifyIcon trayIcon;

        public SPMouse()
        {
            trayIcon = new NotifyIcon();
        }
        static void Main()
        {
            Application.Run(new SPMouse());
        }


        private void resizeCallback(object sender, EventArgs e)
        {
            //if the form is minimized  
            //hide it from the task bar  
            //and show the system tray icon (represented by the NotifyIcon control)  
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                trayIcon.Visible = true;
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

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SPMouse
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "SPMouse";
            this.Load += new System.EventHandler(this.SPMouse_Load);
            this.ResumeLayout(false);

        }

        private void SPMouse_Load(object sender, EventArgs e)
        {

        }
    }
}