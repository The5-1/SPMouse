using System;
using System.Drawing;
using System.Windows.Forms;


namespace SPMouse
{
    public class SPMouse : System.Windows.Forms.Form
    {
        private Icon icon;
        private Color accentColor;
        private Font font_header;
        private Font font;

        private Label label1 = new Label();

        private NotifyIcon tray = new NotifyIcon();
        private ContextMenu tray_menu = new ContextMenu();
        private MenuItem tray_menu_startstop = new MenuItem();
        private MenuItem tray_menu_exit = new MenuItem();


        private SPMouseOverlay overlay;

        static void Main()
        {

#if false
            App.DrawOnScreenTest();
#endif 

            Application.EnableVisualStyles(); //Enable Win10 Styling
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SPMouse());
        }

        public SPMouse()
        {
            //Loads Resources from the Project Properties!
            icon = Properties.Resources.SPM_Icon;
            accentColor = Win32Util.GetThemeColor();
            font_header = SystemFonts.CaptionFont;
            font = SystemFonts.DefaultFont;

            //Main Window
            this.Icon = icon; //load the icon from the project property Resources
            this.Name = "SPMouse";
            this.Text = "SPMouse";
            this.font = font_header;
            this.ClientSize = new System.Drawing.Size(256, 256);
            this.BackColor = accentColor;
            //this.BackColor = SystemColors.Highlight;
            //this.TransparencyKey = this.BackColor;

            this.FormBorderStyle = FormBorderStyle.Sizable;

            //this.AllowTransparency = true;
            //this.SetStyle(ControlStyles.UserPaint, true);
            //this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            //this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            //this.EnableBlur();
            

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

            tray_menu_startstop.Text = "Stop";

            tray_menu_exit.Text = "E&xit";
            tray_menu_exit.Click += new System.EventHandler(this.exit);

            tray.ContextMenu.MenuItems.AddRange( new System.Windows.Forms.MenuItem[]{
                this.tray_menu_startstop,
                this.tray_menu_exit
            });


            //Overlay
            overlay = new SPMouseOverlay();
            overlay.init(icon);
            overlay.Show();

            //Add controls
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                label1
            });

            //Setup callbacks
            this.Resize += this.resizeCallback;
            this.MouseMove += this.mouseMoveCallback;

            this.tray.MouseDoubleClick += this.notifyIconClickedCallback;
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

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SPMouse
            // 
            this.Load += new System.EventHandler(this.SPMouse_Load);
            this.ResumeLayout(false);
        }

        private void SPMouse_Load(object sender, EventArgs e)
        {

        }


        //only called when mouse is inside the Form
        private void mouseMoveCallback(object sender, MouseEventArgs e)
        {
            int mouseX = e.X;
            int mouseY = e.Y;

            MoveCursorTest();
        }

        private void MoveCursorTest()
        {
            var cpy = Cursor.Current;

            //Get a pointer to the current cursor
            this.Cursor = new Cursor(Cursor.Current.Handle);

            //set the position
            Cursor.Position = new Point(960, 560);

            //limit cursor region, defaults to whole screen
            //Cursor.Clip = new Rectangle(this.Location, this.Size);
        }

    }



}