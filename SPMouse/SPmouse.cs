using System;
using System.Drawing;
using System.Windows.Forms;

namespace SPMouse
{
    public class SPMouse : System.Windows.Forms.Form
    {
        public struct Settings
        {
            public float sRopeLength;
            public Color sDarkgray;
            public Color sLightgray;
            public Color sAccent;
        }

        public static SPMouse.Settings settings;

        private bool m_running;

        private PopupUtil m_popup;

        public static Icon icon;
        private Color accentColor;
        private Font font_header;
        private Font font;

        private Label label1 = new Label();

        private NotifyIcon tray = new NotifyIcon();
        private ContextMenu tray_menu = new ContextMenu();
        private MenuItem tray_menu_maximize = new MenuItem();
        private MenuItem tray_menu_startstop = new MenuItem();
        private MenuItem tray_menu_exit = new MenuItem();


        private SPMouseOverlay overlay;
        private InputHandler m_input;

        static void Main()
        {

#if false
            App.DrawOnScreenTest();
#endif 

            SPMouse.Settings settings = new SPMouse.Settings();
            settings.sRopeLength = 128;
            settings.sDarkgray = Color.FromArgb(255, 32, 32, 32);
            settings.sLightgray = Color.FromArgb(255, 128, 128, 128);
            settings.sLightgray = Color.FromArgb(255, 128, 128, 128);
            settings.sAccent = Win32Util.GetThemeColor();

            Application.EnableVisualStyles(); //Enable Win10 Styling
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SPMouse(settings));
        }

        public SPMouse(SPMouse.Settings settings)
        {
            SPMouse.settings = settings;
            //Loads Resources from the Project Properties!
            SPMouse.icon = Properties.Resources.SPM_Icon;

            accentColor = Win32Util.GetThemeColor();
            font_header = SystemFonts.CaptionFont;
            font = SystemFonts.DefaultFont;

            //Main Window
            this.Icon = icon; //load the icon from the project property Resources
            this.Name = "SPMouse";
            this.Text = "SPMouse";
            this.font = font_header;

            this.MaximizeBox = false;
            this.ShowInTaskbar = true; //generally allow showing
            //this.FormBorderStyle = FormBorderStyle.FixedSingle;

            this.ClientSize = new System.Drawing.Size(256, 256);
            this.BackColor = accentColor;
            //this.BackColor = SystemColors.Highlight;
            //this.TransparencyKey = this.BackColor;
            //this.AllowTransparency = true;
            //this.SetStyle(ControlStyles.UserPaint, true);
            //this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            //this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            //this.EnableBlur();


            //Main Window - Label
            label1.Text = "Woah... such precision.\nMuch mousing, so surgical.";
            label1.TextAlign = ContentAlignment.TopCenter;
            label1.Dock = DockStyle.Fill;
            label1.Padding = new Padding(8);

            //Tray Icons
            //tray.BalloonTipClosed += (sender, e) => { var thisIcon = (NotifyIcon)sender; thisIcon.Visible = false; thisIcon.Dispose(); };
            tray.Icon = icon;
            m_popup = new PopupUtil(tray);
            m_popup.addSate("ToTray", "SPMouse is running in the system tray.");
            tray.BalloonTipIcon = ToolTipIcon.Info;
            tray.BalloonTipTitle = "Surgical Precision Mouse";
            tray.BalloonTipText = "Woah... such precision.\nMuch mousing, so surgical.";

            //Tray context Menu: https://docs.microsoft.com/de-de/dotnet/api/system.windows.forms.notifyicon.contextmenu?view=netframework-4.8
            tray.ContextMenu = tray_menu;

            tray_menu_maximize.Text = "Maximize";
            tray_menu_maximize.Click += new System.EventHandler(this.doMaximise);

            tray_menu_startstop.Text = "Stop";
            tray_menu_startstop.Click += new System.EventHandler(this.onToggleRunningButton);

            tray_menu_exit.Text = "E&xit";
            tray_menu_exit.Click += new System.EventHandler(this.onExit);

            tray.ContextMenu.MenuItems.AddRange( new System.Windows.Forms.MenuItem[]{
                this.tray_menu_maximize,
                this.tray_menu_startstop,
                this.tray_menu_exit
            });


            m_input = new InputHandler();

            //Overlay
            overlay = new SPMouseOverlay(icon, m_input);
            overlay.Show();
            
            //Add controls
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                label1
            });

            //Setup Event callbacks
            this.Resize += this.onResize;
            //this.MouseMove += this.mouseMoveCallback;

            this.tray.MouseDoubleClick += this.doMaximise;

            m_running = false;
            toggleRunning();
        }

        private void shutdown()
        {
            tray.Visible = false;
            tray.Dispose();
            this.Dispose();
            Application.Exit();
            System.Environment.Exit(0);
        }

        private void moveToTray()
        {
            this.Hide();
            this.WindowState = FormWindowState.Minimized;
            tray.Visible = true;
        }

        private void moveToWindow()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            tray.Visible = false;
        }

        private void toggleRunning()
        {
            if (!m_running)
            {
                m_input.start();
                tray_menu_startstop.Text = "Stop";
                m_running = true;
                overlay.Visible = true;
            }
            else
            {
                m_input.stop();
                tray_menu_startstop.Text = "Start";
                m_running = false;
                overlay.Visible = false;
            }
        }

        private void onToggleRunningButton(object sender, EventArgs e)
        {
            toggleRunning();
        }

        private void onResize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                moveToTray();
            }
        }

        private void doMaximise(object sender, EventArgs e)
        {
            moveToWindow();
        }

        private void onExit(object sender, EventArgs e)
        {
            shutdown();
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            moveToTray();
            m_popup.pop("ToTray");
        }

        protected override void OnClosed(EventArgs e)
        {
            shutdown();
            base.OnClosed(e);
        }

        //only called when mouse is inside the Form
        /*
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

            //ToolTip hint = new ToolTip();
            //hint.IsBalloon = true;
            //hint.ToolTipIcon = ToolTipIcon.Error;
            //hint.Show("Please create a world.",
        }
        */

    }



}