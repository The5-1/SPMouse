using System;
using System.Drawing;
using System.Windows.Forms;
using System.Numerics;

namespace SPMouse
{
    public class SPMouseOverlay : System.Windows.Forms.Form
    {
        private InputHandler m_input;
        InputHandler.NotifyPosCallaback m_updateCallback;

        private Point cursorPos;
        private Point pullPos;
        private bool drawing;

        private Pen penRope;
        private Pen penRopeA;
        private Pen penRopeB;

        public SPMouseOverlay(Icon icon, InputHandler inputhandler)
        {
            m_input = inputhandler;
            this.ClientSize = new System.Drawing.Size(256, 256);
            this.Name = "SPMouse Overlay";
            this.Text = "SPMouse Overlay";
            this.Icon = icon;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.MinimizeBox = this.MaximizeBox = false;
            this.AllowTransparency = true;
            //this.BackColor = Color.FromArgb(255, 0, 0, 0);
            this.BackColor = Color.Black;
            this.TransparencyKey = this.BackColor;
            this.Opacity = 1.0;
            this.DoubleBuffered = true;
            //this.makeKlicktrough(); //we do it via override CreateParams instead;

            penRope = new Pen(SPMouse.settings.sAccent, 2f);
            penRopeA = new Pen(SPMouse.settings.sDarkgray, 3f);
            penRopeB = new Pen(SPMouse.settings.sLightgray, 3f);

            m_updateCallback = new InputHandler.NotifyPosCallaback(update);
            m_input.registerUpdateCallback(m_updateCallback);

            this.Paint += onRedraw;
        }

        public void update(Point cursorPos, Point pullPos, bool drawing)
        {
            this.cursorPos = cursorPos;
            this.pullPos = pullPos;
            this.drawing = drawing;

            //Update VS Invalidate VS Refresh: https://blogs.msdn.microsoft.com/subhagpo/2005/02/22/whats-the-difference-between-control-invalidate-control-update-and-control-refresh/
            this.Invalidate(); //invalidates a region and queues it for redrawing
            //this.Invalidate(aabb); //this does not work if you shake the mouse fast

            //this.Refresh(); //immediately, may have worse performance
            //this.Update(); //invalidates all and refreshes immediately
        }

        public void onRedraw(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
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


            /* Bezier length approximation https://stackoverflow.com/questions/29438398/cheap-way-of-calculating-cubic-bezier-length
            ** Bezier bezier = Bezier (p0, p1, p2, p3);
            ** chord = (p3-p0).Length;
            ** cont_net = (p0 - p1).Length + (p2 - p1).Length + (p3 - p2).Length;
            ** app_arc_length = (cont_net + chord) / 2
            ** --> rope   = spanning quad / 2
            ** --> rope*2 = spanning quad
            **
            ** Inverse problem: find controll points for given length:
            ** our spanning quad is a rectangle, we know the width = dist, we seek the height:
            ** --> spanning rectangle = (2*width + 2*height)
            ** --> rope*2 = spanning rectangle
            ** --> rope*2 = (2*dist + 2*height)
            ** --> (rope*2 - dist*2) /2 = height
            ** --> rope - dist = height
            */

            float diff = Vector2.Distance(VectorUtil.toVec(cursorPos), VectorUtil.toVec(pullPos));
            int hangtrough = (int)(Math.Max(0.0, SPMouse.settings.sRopeLength - diff) * 0.666); //empiric factor that makes it work

            //g.DrawBezier(penRopeA, cursorPos.X, cursorPos.Y,
            //                        cursorPos.X, cursorPos.Y + hangtrough,
            //                        pullPos.X, pullPos.Y + hangtrough,
            //                        pullPos.X, pullPos.Y
            //            );
            g.DrawBezier(penRope, cursorPos.X, cursorPos.Y,
                                    cursorPos.X, cursorPos.Y + hangtrough,
                                    pullPos.X, pullPos.Y + hangtrough,
                                    pullPos.X, pullPos.Y
                        );

        }

        //Manged way to use "SetWindowLong" without native call!
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // turn on WS_EX_TOOLWINDOW style bit
                cp.ExStyle |= Win32Util.WS_EX_LAYERED | Win32Util.WS_EX_TRANSPARENT | Win32Util.WS_EX_TOOLWINDOW | 0x00000008;
                return cp;
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);

        }
    }
}