using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading.Tasks;


public class PopupUtil
{
    protected struct PopupState
    {
        public string text;
        public ToolTipIcon icon;

        public PopupState(string text, ToolTipIcon icon = ToolTipIcon.None)
        {
            this.text = text;
            this.icon = icon;
        }
    }

    private NotifyIcon m_tray;

    private Dictionary<string,PopupState> states = new Dictionary<string,PopupState>();

    public PopupUtil(NotifyIcon tray)
    {
        m_tray = tray;
    }

    public void pop(string state, int dur = 1000)
    {
        m_tray.BalloonTipText = states[state].text;
        m_tray.BalloonTipIcon = states[state].icon;
        m_tray.ShowBalloonTip(dur);
    }

    public void addSate(string name, string text, ToolTipIcon icon = ToolTipIcon.None)
    {
        states.Add(name, new PopupState(text,icon));
    }


#if false
    //This pollutes the tray and generally looks dangerous
    public static void show(string title, string message, Icon icon = null, ToolTipIcon type = ToolTipIcon.None, int duration = 1000, int deletetime = 15000)
    {
        NotifyIcon popup = new NotifyIcon();
        popup.Icon = icon == null ? SystemIcons.Information : icon;
        popup.Text = title;
        popup.Visible = true;
        popup.BalloonTipIcon = type;
        popup.BalloonTipTitle = title;
        popup.BalloonTipText = message;
        popup.ShowBalloonTip(duration);
        Task.Delay(deletetime).ContinueWith(t => popup.Dispose());
    }
#endif

}