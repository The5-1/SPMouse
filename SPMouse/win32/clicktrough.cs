using System.Windows.Forms;

public static partial class WindowExtension
{

    // https://stackoverflow.com/questions/1536141/how-to-draw-directly-on-the-windows-desktop-c/37707278#37707278
    public static void makeKlicktrough(this Form @this)
    {
        Win32Util.SetWindowLong(@this.Handle, Win32Util.GWL_EXSTYLE, Win32Util.GetWindowLong(@this.Handle, Win32Util.GWL_EXSTYLE) | Win32Util.WS_EX_LAYERED | Win32Util.WS_EX_TRANSPARENT);
    }

}