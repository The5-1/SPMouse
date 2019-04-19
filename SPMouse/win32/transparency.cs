using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using System.Windows.Forms;

public static partial class Win32Util
{
    //https://stackoverflow.com/questions/51370670/creating-windows-10-transparency-effects-in-c-sharp-form

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);
    public struct WindowCompositionAttributeData
    {
        public WindowCompositionAttribute Attribute;
        public IntPtr Data;
        public int SizeOfData;
    }

    public enum WindowCompositionAttribute
    {
        WCA_ACCENT_POLICY = 19
    }

    public enum AccentState
    {
        ACCENT_DISABLED = 0,
        ACCENT_ENABLE_GRADIENT = 1,
        ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
        ACCENT_ENABLE_BLURBEHIND = 3,
        ACCENT_INVALID_STATE = 4
    }

    public struct AccentPolicy
    {
        public AccentState AccentState;
        public int AccentFlags;
        public int GradientColor;
        public int AnimationId;
    }
}

public static class WindowExtension
{

    // https://www.codeproject.com/Answers/1174965/How-can-I-make-my-form-like-windows-blurr-effect-o#answer3
    public static void EnableBlur(this Form @this)
    {
        var accent = new Win32Util.AccentPolicy();
        accent.AccentState = Win32Util.AccentState.ACCENT_ENABLE_BLURBEHIND;
        var accentStructSize = Marshal.SizeOf(accent);
        var accentPtr = Marshal.AllocHGlobal(accentStructSize);
        Marshal.StructureToPtr(accent, accentPtr, false);
        var Data = new Win32Util.WindowCompositionAttributeData();
        Data.Attribute = Win32Util.WindowCompositionAttribute.WCA_ACCENT_POLICY;
        Data.SizeOfData = accentStructSize;
        Data.Data = accentPtr;
        Win32Util.SetWindowCompositionAttribute(@this.Handle, ref Data);
        Marshal.FreeHGlobal(accentPtr);
    }

}