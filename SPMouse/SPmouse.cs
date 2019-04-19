using System;
using System.Drawing;
using System.Windows.Forms;

using FormsExtensions;

namespace SPMouse
{
    public class SPMouse : System.Windows.Forms.Form
    {
        private Icon icon;

        private Label label1 = new Label();


        private NotifyIcon tray = new NotifyIcon();
        private ContextMenu tray_menu = new ContextMenu();
        private MenuItem tray_menu_exit = new MenuItem();

        public SPMouse()
        {
            icon = Properties.Resources.SPM_Icon;

            //Main Window
            this.Icon = icon; //load the icon from the project property Resources
            this.Name = "SPMouse";
            this.Text = "SPMouse";
            this.ClientSize = new System.Drawing.Size(256, 256);
            this.BackColor = Color.Magenta;
            this.TransparencyKey = Color.Magenta;

            this.AllowTransparency = true;
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.EnableBlur();
            

            //Main Window - Label
            label1.Text = "ASDF";
            label1.Location = new System.Drawing.Point(10, 10);
            label1.Size = new System.Drawing.Size(10, 10);

            //Tray Icons
            tray.Icon = icon;
            tray.BalloonTipIcon = ToolTipIcon.Info;
            tray.BalloonTipTitle = "Surgical Precision Mouse";
            tray.BalloonTipText = "Minimized to system tray.";

            //Tray context Menu: https://docs.microsoft.com/de-de/dotnet/api/system.windows.forms.notifyicon.contextmenu?view=netframework-4.8
            tray.ContextMenu = tray_menu;

            tray_menu_exit.Text = "E&xit";
            tray_menu_exit.Click += new System.EventHandler(this.exit);

            tray.ContextMenu.MenuItems.AddRange( new System.Windows.Forms.MenuItem[]{
                this.tray_menu_exit
            });


            //Add controls
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                label1
            });

            //Setup callbacks
            this.Resize += this.resizeCallback;
            this.tray.MouseDoubleClick += this.notifyIconClickedCallback;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Load += new System.EventHandler(this.loadCallback);
            this.ResumeLayout(false);
        }

        //Main Entry Point
        static void Main()
        {
            Application.Run(new SPMouse());
        }

        private void loadCallback(object sender, EventArgs e)
        {

        }

        private void resizeCallback(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                tray.Visible = true;
                tray.ShowBalloonTip(500);
                this.Hide();
            }

            else if (this.WindowState == FormWindowState.Normal)
            {
                tray.Visible = false;
            }
        }

        private void notifyIconClickedCallback(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void exit(object sender, EventArgs e)
        {
            tray.Dispose();
            this.Dispose();
            Application.Exit();
            System.Environment.Exit(0);
        }

        //==========================================================================
        private void Action1Click(Object sender, EventArgs e)
        {
            // nur als Beispiel:
            // new MyForm ().Show ();
        }


    }
}