using System;
using System.Drawing;
using System.Windows.Forms;

namespace SPMouse
{
    public class SPMouseOverlay : System.Windows.Forms.Form
    {
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SPMouseOverlay
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "SPMouseOverlay";
            this.Load += new System.EventHandler(this.SPMouseOverlay_Load);
            this.ResumeLayout(false);

        }

        private void SPMouseOverlay_Load(object sender, EventArgs e)
        {

        }
    }
}