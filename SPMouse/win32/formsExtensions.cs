using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Win32Util;

namespace FormsExtensions
{
    public static class WindowExtension
    {
        public static void EnableBlur(this Form @this)
        {
            var accent = new UnsafeNativeMethods.AccentPolicy();
            accent.AccentState = UnsafeNativeMethods.AccentState.ACCENT_ENABLE_BLURBEHIND;
            var accentStructSize = Marshal.SizeOf(accent);
            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);
            var Data = new UnsafeNativeMethods.WindowCompositionAttributeData();
            Data.Attribute = UnsafeNativeMethods.WindowCompositionAttribute.WCA_ACCENT_POLICY;
            Data.SizeOfData = accentStructSize;
            Data.Data = accentPtr;
            UnsafeNativeMethods.SetWindowCompositionAttribute(@this.Handle, ref Data);
            Marshal.FreeHGlobal(accentPtr);
        }

    }
}