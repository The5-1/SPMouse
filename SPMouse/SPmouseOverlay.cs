using System;
using System.Drawing;
using System.Windows.Forms;

namespace SPMouse
{
    public class SPMouseOverlay : System.Windows.Forms.Form
    {
        private InputHandler m_input;
        InputHandler.NotifyPosCallaback m_updateCallback;

        private Point cursorPos;
        private Point pullPos;
        public SPMouseOverlay(Icon icon, InputHandler inputhandler)
        {
            m_input = inputhandler;
            this.ClientSize = new System.Drawing.Size(256, 256);
            this.Name = "SPMouse Overlay";
            this.Text = "SPMouse Overlay";
            this.Icon = icon;
            this.Load += new System.EventHandler(this.SPMouseOverlay_Load);
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.AllowTransparency = true;
            this.BackColor = Color.FromArgb(255, 0, 0, 0);
            this.TransparencyKey = this.BackColor;
            this.Opacity = 1;
            this.DoubleBuffered = true;

            this.makeKlicktrough();

            m_updateCallback = new InputHandler.NotifyPosCallaback(update);
            m_input.registerUpdateCallback(m_updateCallback);

            this.Paint += onRedraw;
        }

        public void update(Point cursorPos, Point pullPos)
        {
            this.cursorPos = cursorPos;
            this.pullPos = pullPos;

            //Update VS Invalidate VS Refresh: https://blogs.msdn.microsoft.com/subhagpo/2005/02/22/whats-the-difference-between-control-invalidate-control-update-and-control-refresh/
            this.Invalidate(); //invalidates a region and queues it for redrawing
            //this.Invalidate(aabb); //this does not work if you shake the mouse fast

            //this.Refresh(); //immediately, may have worse performance
            //this.Update(); //invalidates all and refreshes immediately
        }

        public void onRedraw(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.AssumeLinear;
            int padding = 10;
            int minx = (int)Math.Min(cursorPos.X, pullPos.X) - padding;
            int miny = (int)Math.Min(cursorPos.Y, pullPos.Y) - padding;
            int maxx = (int)Math.Max(cursorPos.X, pullPos.X) - minx + padding * 2;
            int maxy = (int)Math.Max(cursorPos.Y, pullPos.Y) - miny + padding * 2;
            Rectangle aabb = new Rectangle(minx, miny, maxx, maxy);

            //Rectangle aabb = new Rectangle(minx, miny, maxx, maxy);
            //g.Clear(Color.Transparent);
            //g.Clip = new Region(aabb);
            //g.FillRegion(Brushes.Black, g.Clip);

            g.DrawBezier(Pens.Red, cursorPos, cursorPos, pullPos, pullPos);
        }

        //Hide the form from Alt+Tab by making it a "Tool Window"
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // turn on WS_EX_TOOLWINDOW style bit
                cp.ExStyle |= 0x80000 | 0x20 | 0x80 | 0x00000008;
                return cp;
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);

        }

        private void SPMouseOverlay_Load(object sender, EventArgs e)
        {

        }
    }
}