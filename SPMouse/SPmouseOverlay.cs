using System;
using System.Drawing;
using System.Windows.Forms;

namespace SPMouse
{
    public class SPMouseOverlay : System.Windows.Forms.Form
    {
        private InputHandler m_input;
        InputHandler.NotifyPosCallaback m_updateCallback;
        Graphics g;
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
            //this.TransparencyKey = this.BackColor;
            this.Opacity = 0.5;

            this.makeKlicktrough();

            m_updateCallback = new InputHandler.NotifyPosCallaback(update);
            g = this.CreateGraphics();
            m_input.registerUpdateCallback(m_updateCallback);
        }

        public void update(Point cursorPos, Point pullPos)
        {
            g.DrawBezier(Pens.Red, cursorPos, cursorPos, pullPos, pullPos);
        }

        //Hide the form from Alt+Tab by making it a "Tool Window"
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // turn on WS_EX_TOOLWINDOW style bit
                cp.ExStyle |= 0x80;
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